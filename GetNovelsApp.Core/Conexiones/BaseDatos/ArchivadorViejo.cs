using System;
using System.Collections.Generic;
using System.Data;
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
        public INovela MeteNovelaDB(Uri LinkNovela, out bool YaExiste)
        {
            YaExiste = NovelaExisteEnDB(LinkNovela);
            if (!YaExiste)
            {
                using IDbConnection cnn = DataBaseAccess.GetConnection();

                //Obteniendo toda la info de la web y metiendola en la DB:
                InformacionNovelaDB novDBInfo = InsertaNovelaEnDB(LinkNovela, cnn, out InformacionNovelaOnline infoNov);

                //Capitulos:
                List<Capitulo> CapitulosNovela = GetNovelsFactory.FabricaCapitulos(infoNov.LinksDeCapitulos);
                GuardaCapitulos(CapitulosNovela, novDBInfo.ID); //Itera los caps y encuentra su info.

                //Regresando una novela para runtime:
                INovela nov = GetNovelsFactory.ObtenNovela(CapitulosNovela, novDBInfo);
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
        private InformacionNovelaDB InsertaNovelaEnDB(Uri LinkNovela, IDbConnection cnn, out InformacionNovelaOnline infoNov)
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

    }
}
