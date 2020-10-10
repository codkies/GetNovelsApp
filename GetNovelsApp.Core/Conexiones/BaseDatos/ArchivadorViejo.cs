using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Utilidades;

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
        public INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> Legacy_MeteNovelaDB(Uri LinkNovela, out bool YaExiste)
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
                INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> nov = GetNovelsFactory.FabricaNovela(CapitulosNovela, novDBInfo);
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
            string qry = Legacy_InsertNovel_Query(infoNov);
            cnn.Execute(qry);

            //Insertando las tagas
            string qID = $"select ID from {TablaNovelas} where LinkPrincipal = \"{LinkNovela}\" ";
            int novelID = cnn.Query<int>(qID).First();
            string qry_tags = Legacy_InsertTags_Query(infoNov, novelID);
            cnn.Execute(qry_tags);


            //Obteniendo una referencia de la DB
            string query_ObtenDBInfo = $"SELECT n.id, n.Titulo, n.LinkPrincipal, n.Sipnosis, n.Imagen, t.Tags " +
                                        $"from {TablaNovelas} as n " +
                                        $"join {TablaClasificacion} as t " +
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
                    var qry = Legacy_InsertCapitulo_Query(novelaID, c);
                    cnn.Execute(qry);
                    first = true;
                }
                catch (SQLiteException)
                {
                    var updateQry = Legacy_UpdateCapitulo_Query(novelaID, c);
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



        public IEnumerable<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>> Legacy_ObtenTodasNovelas()
        {
            List<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>> output = new List<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>>();
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            //Obteniendo las informaciones de novela:
            string getThemAll = Legacy_GetAllNovels_Query();
            IEnumerable<InformacionNovelaDB> InfosDeNovelas = cnn.Query<InformacionNovelaDB>(getThemAll);

            //Obteniendo los capitulos de todas:
            foreach (var InfoNov in InfosDeNovelas)
            {
                string getThemChapters = Legacy_GetChaptersOfNovel_Query(InfoNov);
                var Capitulos = cnn.Query<Capitulo>(getThemChapters);
                output.Add(GetNovelsFactory.FabricaNovela(Capitulos, InfoNov));
            }

            cnn.Dispose();
            return output;
        }


        #region Legacy Queries

        /// <summary>
        /// Query para obtener info de una novela en la tabla de novelas.
        /// </summary>
        /// <param name="infoDBNovela"></param>
        /// <returns></returns>
        private static string Legacy_GetChaptersOfNovel_Query(InformacionNovelaDB infoDBNovela)
        {
            return $"select Link, TextoCapitulo, Titulo, Numero, Valor from {TablaCapitulos} where NovelaID = '{infoDBNovela.ID}'";
        }


        /// <summary>
        /// Query para obtener una InformacionNovelaDB según el ID de la novela.
        /// </summary>
        /// <param name="novelID"></param>
        /// <returns></returns>
        private static string Legacy_GetNovDBInfoWithID_Query(int novelID)
        {
            return $"SELECT n.id, n.Titulo, n.LinkPrincipal, n.Sipnosis, n.Imagen, t.Tags " +
                                        $"from {TablaNovelas} as n " +
                                        $"join {TablaClasificacion} as t " +
                                        $"on n.ID = t.NovelaID and n.id = {novelID}";
        }


        /// <summary>
        /// Query para obtener todas las novelas en formato de InformacionNovelaDB.
        /// </summary>
        /// <returns></returns>
        private string Legacy_GetAllNovels_Query()
        {
            return $"SELECT n.id, n.Titulo, n.LinkPrincipal, n.Sipnosis, n.Imagen, t.Tags " +
                    $"FROM {TablaNovelas} AS n " +
                    $"left JOIN {TablaClasificacion} AS t " +
                        $"on n.ID = t.NovelaID";
        }


        /// <summary>
        /// Query para obtener el ID de una novela acorde a su Link
        /// </summary>
        /// <param name="LinkNovela"></param>
        /// <returns></returns>
        private string Legacy_GetIDNovel_Query(Uri LinkNovela)
        {
            return $"select n.NovelaID from {i.TNovelas} as n " +
                $"join {i.TLinks} as l where l.Link = '{LinkNovela}'";
        }


        /// <summary>
        /// Query para insertar una novela en la DB
        /// </summary>
        /// <param name="infoNov"></param>
        /// <returns></returns>
        private string Legacy_InsertNovel_Query(InformacionNovelaOnline infoNov)
        {
            return $"insert into {i.TNovelas} " +
                    $"(Titulo, LinkPrincipal, Sipnosis, Imagen) values" +
                    $"('{infoNov.Titulo}', '{infoNov.LinkPrincipal}', '{infoNov.Sipnosis}', '{infoNov.Imagen}')";
        }


        /// <summary>
        /// Query para insertar los tags de una novela en la DB.
        /// </summary>
        /// <param name="infoNov"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string Legacy_InsertTags_Query(InformacionNovelaOnline infoNov, int ID)
        {
            return $"insert into {TablaClasificacion} " +
                    $"(NovelaID, Tags) values" +
                    $"('{ID}', '{ManipuladorStrings.TagsEnString(infoNov.Tags)}')";
        }


        /// <summary>
        /// Toma un capitulo vacio, obtiene su info basica y la mete en la DB.
        /// </summary>
        private string Legacy_InsertCapitulo_Query(int novelaID, Capitulo c)
        {
            return $"insert into {TablaCapitulos} " +
                    $"(NovelaID, Link, TextoCapitulo, Numero, Titulo, Valor) values" +
                    $"('{novelaID}', '{c.Link}', \"{c.Texto}\", '{c.NumeroCapitulo}', '{c.TituloCapitulo}', '{c.Valor}')";
        }


        /// <summary>
        /// Query para actualizar un capitulo en la DB.
        /// </summary>
        /// <param name="novelaID"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private string Legacy_UpdateCapitulo_Query(int novelaID, Capitulo c)
        {
            return $"update {TablaCapitulos} " +
                    $"set TextoCapitulo = \"{c.Texto}\" " +
                    $"where NovelaID = '{novelaID}' and Link = '{c.Link}'";
        }

        #endregion
    }
}
