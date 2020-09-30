using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Security.Permissions;
using Dapper;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.Core.Conexiones.DB
{
    /// <summary>
    /// Query maker
    /// </summary>
    public class Archivador : IReportero
    {
        private const string TablaNovelas = "Novelas";
        private const string TablaCapitulos = "Capitulos";
        private const string TablaTags = "Tags";


        public string Nombre => "DB";

        #region Core


        /// <summary>
        /// Regresa una instancia de una novela registrada en la base de datos.
        /// </summary>
        /// <param name="infoNov"></param>
        /// <returns></returns>
        public INovela BuscaNovelaEnDB(Uri LinkNovela) 
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            string obtenIDnovela = GetIDNovel_Query(LinkNovela);
            var _ = cnn.Query<int>(obtenIDnovela);

            bool NoExisteLaNovela = _.Any() == false;
            if (NoExisteLaNovela)
            {
                //Obteniendo toda la info de la web y metiendola en la DB:
                InformacionNovelaDB novDBInfo = CreateNovel(LinkNovela, cnn, out InformacionNovelaOnline infoNov);

                //Capitulos:
                List<Capitulo> CapitulosNovela = CreaCapitulos(infoNov.LinksDeCapitulos);
                GuardaCapitulos(CapitulosNovela, novDBInfo.ID); //Itera los caps y encuentra su info.

                //Regresando una novela para runtime:
                INovela nov = GetNovelsFactory.ObtenNovela(CapitulosNovela, novDBInfo);
                cnn.Dispose();
                return nov;
            }
            else
            {
                cnn.Dispose();
                return SacaNovelaDB(LinkNovela);
            }
        }


        public void GuardaCapitulos(List<Capitulo> capitulosVacios, int novelaID)
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
                catch (Exception)
                {
                    continue;
                }
            }

            if(first)
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

       

        private INovela SacaNovelaDB(Uri LinkNovela) //Debe ir privada.
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            //Encuentra la infobasica
            string qryNovela = $"select * from {TablaNovelas} where LinkPrincipal = '{LinkNovela}'";
            InformacionNovelaDB infoDBNovela = cnn.Query<InformacionNovelaDB>(qryNovela).First();

            //Encuentra los capitulos
            string qryCapitlos = $"select Link, TextoCapitulo, Titulo, Numero, Valor from {TablaCapitulos} where NovelaID = '{infoDBNovela.ID}'";
            List<Capitulo> Capitulos = cnn.Query<Capitulo>(qryCapitlos).ToList();

            INovela novela = GetNovelsFactory.ObtenNovela(Capitulos, infoDBNovela);

            cnn.Dispose();
            return novela;
        }

        #endregion


        #region Queries


        private string GetIDNovel_Query(Uri LinkNovela)
        {
            return $"select ID from {TablaNovelas} where LinkPrincipal = '{LinkNovela}'";
        }


        private string InsertNovel_Query(InformacionNovelaOnline infoNov)
        {
            return $"insert into {TablaNovelas} " +
                    $"(Titulo, LinkPrincipal, Sipnosis, Imagen) values" +
                    $"('{infoNov.Titulo}', '{infoNov.LinkPrincipal}', '{infoNov.Sipnosis}', '{infoNov.Imagen}')";
        }

        private string InsertTags_Query(InformacionNovelaOnline infoNov, int ID)
        {
            return $"insert into {TablaTags} " +
                    $"(NovelaID, Tags) values" +
                    $"('{ID}', '{TagsEnString(infoNov.Tags)}')";
        }

        /// <summary>
        /// Toma un capitulo vacio, obtiene su info basica y la mete en la DB.
        /// </summary>
        private string InsertCapitulo_Query(int novelaID, Capitulo c)
        {
            return  $"insert into {TablaCapitulos} " +
                    $"(NovelaID, Link, TextoCapitulo, Numero, Titulo, Valor) values" +
                    $"('{novelaID}', '{c.Link}', \"{c.Texto}\", '{c.NumeroCapitulo}', '{c.TituloCapitulo}', '{c.Valor}')";
        }

        private string UpdateCapitulo_Query(int novelaID, Capitulo c)
        {
            return  $"update {TablaCapitulos} " +
                    $"set TextoCapitulo = \"{c.Texto}\" " +
                    $"where NovelaID = '{novelaID}' and Link = '{c.Link}'";
        }

        #endregion



        #region Helpers


        /// <summary>
        /// Crea una novela en la DB. Regresa un NovelDBmodel y un out NovelaWebModel
        /// </summary>
        private InformacionNovelaDB CreateNovel(Uri LinkNovela, IDbConnection cnn, out InformacionNovelaOnline infoNov)
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


        private List<Capitulo> CreaCapitulos(List<Uri> ListaDeLinks)
        {
            List<Capitulo> Capitulos = new List<Capitulo>();
            foreach (Uri link in ListaDeLinks)
            {
                CapituloWebModel _ = ManipuladorDeLinks.EncuentraInformacionCapitulo(link);
                Capitulo capitulo = new Capitulo(_);
                Capitulos.Add(capitulo);
            }
            return Capitulos;
        }


        /// <summary>
        /// Une las tags en una oracion con comas.
        /// </summary>
        /// <param name="ListaDeTags"></param>
        /// <returns></returns>
        private string TagsEnString(List<string> ListaDeTags)
        {
            return string.Join(", ", ListaDeTags);
        }


        /// <summary>
        /// Separa una oracion de tags en una lista.
        /// </summary>
        /// <param name="Tags"></param>
        /// <returns></returns>
        private List<string> TagsEnLista(string Tags)
        {
            return Tags.Split(',').ToList();
        }


        #endregion



        #region Legacy


        /// <summary>
        /// Crea una novela en la DB. Regresa un NovelDBmodel y un out NovelaWebModel
        /// </summary>
        /// <param name="LinkNovela"></param>
        /// <param name="cnn"></param>
        /// <param name="infoNov"></param>
        /// <returns></returns>
        private InformacionNovelaDB Legacy_CreateNovel(Uri LinkNovela, IDbConnection cnn, out InformacionNovelaOnline infoNov)
        {
            //Encontrando informacion de la web.
            infoNov = ManipuladorDeLinks.EncuentraInformacionNovela(LinkNovela);

            string qry = InsertNovel_Query(infoNov);
            cnn.Execute(qry);

            int id = (int)DataBaseAccess.LastID;
            string qry_tags = InsertTags_Query(infoNov, id);
            cnn.Execute(qry_tags);

            //Obteniendo un modelo de la DB.
            string query_ObtenDBInfo = $"select * from {TablaNovelas} where LinkPrincipal = '{LinkNovela}'";
            InformacionNovelaDB novDBInfo = cnn.Query<InformacionNovelaDB>(query_ObtenDBInfo).First();

            //Regresando el modelo de la DB.
            return novDBInfo;
        }



        #endregion
    }
}
