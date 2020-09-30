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
    /// Query maker
    /// </summary>
    public class Archivador : IReportero
    {
        private const string TablaNovelas = "Novelas";
        private const string TablaCapitulos = "Capitulos";

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


        private static string GetIDNovel_Query(Uri LinkNovela)
        {
            return $"select ID from {TablaNovelas} where LinkPrincipal = '{LinkNovela}'";
        }


        private static string InsertNovel_Query(InformacionNovelaOnline infoNov)
        {
            return $"insert into {TablaNovelas} " +
                    $"(Titulo, LinkPrincipal) values" +
                    $"('{infoNov.Titulo}', '{infoNov.LinkPrincipal}')";
        }


        /// <summary>
        /// Toma un capitulo vacio, obtiene su info basica y la mete en la DB.
        /// </summary>
        /// <param name="novelaID"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private string InsertCapitulo_Query(int novelaID, Capitulo c)
        {
            return  $"insert into {TablaCapitulos} " +
                    $"(NovelaID, Link, TextoCapitulo, Numero, Titulo, Valor) values" +
                    $"('{novelaID}', '{c.Link}', \"{c.Texto}\", '{c.NumeroCapitulo}', '{c.TituloCapitulo}', '{c.Valor}')";
        }

        private static string UpdateCapitulo_Query(int novelaID, Capitulo c)
        {
            return  $"update {TablaCapitulos} " +
                    $"set TextoCapitulo = \"{c.Texto}\" " +
                    $"where NovelaID = '{novelaID}' and Link = '{c.Link}'";
        }

        #endregion



        #region Helpers

        /// <summary>
        /// Devuelve true or false si un capitulo tiene texto en la DB.
        /// </summary>
        /// <param name="novelaID"></param>
        /// <param name="cnn"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool RevisaSiCapTieneTexto_Output(int novelaID, IDbConnection cnn, Capitulo c)
        {
            string query_RevisaCap = $"select TextoCapitulo from {TablaCapitulos} where NovelaID = '{novelaID}' and Link = '{c.Link}'";
            string output_RevisaCap = cnn.Query<string>(query_RevisaCap).First();
            bool CapTieneTexto = string.IsNullOrEmpty(output_RevisaCap);
            return CapTieneTexto;
        }


        /// <summary>
        /// Crea una novela en la DB. Regresa un NovelDBmodel y un out NovelaWebModel
        /// </summary>
        /// <param name="LinkNovela"></param>
        /// <param name="cnn"></param>
        /// <param name="infoNov"></param>
        /// <returns></returns>
        private InformacionNovelaDB CreateNovel(Uri LinkNovela, IDbConnection cnn, out InformacionNovelaOnline infoNov)
        {
            //Encontrando informacion de la web.
            infoNov = ManipuladorDeLinks.EncuentraInformacionNovela(LinkNovela);

            string qry = InsertNovel_Query(infoNov);
            cnn.Execute(qry);

            //Obteniendo un modelo de la DB.
            string query_ObtenDBInfo = $"select * from {TablaNovelas} where LinkPrincipal = '{LinkNovela}'";
            InformacionNovelaDB novDBInfo = cnn.Query<InformacionNovelaDB>(query_ObtenDBInfo).First();

            //Regresando el modelo de la DB.
            return novDBInfo;
        }

        /// <summary>
        /// Revisa si un capitulo existe.
        /// </summary>
        /// <param name="novelaID"></param>
        /// <param name="cnn"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private bool RevisaSiCapExiste_Output(int novelaID, IDbConnection cnn, Capitulo c)
        {
            //Comprueba la existencia del cap
            string qry_RevisaCapituloExiste = $"select Link from {TablaCapitulos} where NovelaID = '{novelaID}' and Link = '{c.Link}'";
            var output_capituloExiste = cnn.Query<string>(qry_RevisaCapituloExiste);
            bool CapExiste = output_capituloExiste.Any();
            return CapExiste;
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


        private DataTable CapitulosDataTable(List<Capitulo> Capitulos, int NovelaID)
        {
            var output = new DataTable();

            output.Columns.Add("NovelaID", typeof(Int64));
            output.Columns.Add("TextoCapitulo", typeof(string));
            output.Columns.Add("Numero", typeof(Int64));
            output.Columns.Add("Titulo", typeof(string));
            output.Columns.Add("Link", typeof(string));
            output.Columns.Add("Valor", typeof(Int64));


            foreach (Capitulo c in Capitulos)
            {
                output.Rows.Add(NovelaID, c.Texto, c.NumeroCapitulo, c.TituloCapitulo, c.Link, c.Valor);
            }

            return output;
        }

        #endregion

    }
}
