using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.Core.Conexiones.DB
{
    /// <summary>
    /// Metodos "viejos" de console UI
    /// </summary>
    public partial class Archivador : IReportero
    {
        /// <summary>
        /// Regresa una instancia de una novela registrada en la base de datos.
        /// </summary>
        /// <param name="infoNov"></param>
        /// <returns></returns>
        public INovela Legacy_MeteNovelaDB(Uri LinkNovela, out bool YaExiste)
        {
            YaExiste = NovelaExisteEnDB(LinkNovela);
            if (!YaExiste)
            {
                using IDbConnection cnn = DataBaseAccess.GetConnection();

                //Obteniendo toda la info de la web y metiendola en la DB:
                InformacionNovelaDB novDBInfo = Legacy_InsertaNovelaEnDB(LinkNovela, cnn, out InformacionNovelaOnline infoNov);

                //Capitulos:
                List<Capitulo> CapitulosNovela = GetNovelsFactory.FabricaCapitulos(infoNov.LinksDeCapitulos);
                Legacy_GuardaCapitulos(CapitulosNovela, novDBInfo.ID); //Itera los caps y encuentra su info.

                //Regresando una novela para runtime:
                INovela nov = GetNovelsFactory.FabricaNovela(CapitulosNovela, novDBInfo);
                cnn.Dispose();
                return nov;
            }
            else
            {
                return SacaNovelaDB(LinkNovela);
            }
        }


        /// <summary>
        /// Crea una novela en la DB. Regresa un NovelDBmodel y un out NovelaWebModel
        /// </summary>
        private InformacionNovelaDB Legacy_InsertaNovelaEnDB(Uri LinkNovela, IDbConnection cnn, out InformacionNovelaOnline infoNov)
        {
            //Encontrando informacion de la web.
            infoNov = ManipuladorDeLinks.EncuentraInformacionNovela(LinkNovela);

            //Insertando la novela
            string qry = InsertNovel_Query(infoNov);
            cnn.Execute(qry);

            //Insertando las tagas
            string qID = $"select ID from {TablaNovelas} where LinkPrincipal = \"{LinkNovela}\" ";
            int novelID = cnn.Query<int>(qID).First();
            string qry_tags = InsertTags_Query(infoNov, novelID);
            cnn.Execute(qry_tags);


            //Obteniendo una referencia de la DB
            string query_ObtenDBInfo = $"SELECT n.id, n.Titulo, n.LinkPrincipal, n.Sipnosis, n.Imagen, t.Tags " +
                                        $"from {TablaNovelas} as n " +
                                        $"join {TablaTags} as t " +
                                        $"on n.ID = t.NovelaID and n.id = {novelID}";

            InformacionNovelaDB novDBInfo = cnn.Query<InformacionNovelaDB>(query_ObtenDBInfo).First();


            return novDBInfo;
        }


        public void Legacy_GuardaCapitulos(List<Capitulo> capitulosVacios, int novelaID)
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            bool first = false;
            foreach (Capitulo c in capitulosVacios)
            {
                try
                {
                    var qry = InsertCapitulo_Query(novelaID, c);
                    cnn.Execute(qry);
                    first = true;
                }
                catch (SQLiteException)
                {
                    var updateQry = UpdateCapitulo_Query(novelaID, c);
                    cnn.Execute(updateQry);
                }
                catch (Exception ex)
                {
                    GetNovelsComunicador.ReportaError($"Error: {ex.Message}", this);
                    continue;
                }
            }

            if (first)
            {
                string Mensaje = $"Links de {capitulosVacios.First().TituloCapitulo} a {capitulosVacios.Last().TituloCapitulo} obtenidos.";
                GetNovelsComunicador.Reporta(Mensaje, this);
            }
            else
            {
                string Mensaje = $"Textos de {capitulosVacios.First().TituloCapitulo} a {capitulosVacios.Last().TituloCapitulo} obtenidos.";
                GetNovelsComunicador.Reporta(Mensaje, this);
            }

            cnn.Dispose();
        }



        public IEnumerable<INovela> Legacy_ObtenTodasNovelas()
        {
            List<INovela> output = new List<INovela>();
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            //Obteniendo las informaciones de novela:
            string getThemAll = GetAllNovels_Query();
            IEnumerable<InformacionNovelaDB> InfosDeNovelas = cnn.Query<InformacionNovelaDB>(getThemAll);

            //Obteniendo los capitulos de todas:
            foreach (var InfoNov in InfosDeNovelas)
            {
                string getThemChapters = GetChaptersOfNovel_Query(InfoNov);
                var Capitulos = cnn.Query<Capitulo>(getThemChapters);
                output.Add(GetNovelsFactory.FabricaNovela(Capitulos, InfoNov));
            }

            cnn.Dispose();
            return output;
        }


    }
}
