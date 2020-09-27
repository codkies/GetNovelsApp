using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using Dapper;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.GetNovelsApp.Empaquetador.BaseDatos
{
    /*
     Las novelas tienen lista de capitulso impresos y no impresos. Y el empaquetador trabaja con estas listas... 
     */

    public class DataBaseAccess
    {
        public static string GetConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }

        public static IDbConnection GetConnection()
        {
            return new SQLiteConnection(GetConnectionString(), true);
        }
    }

    /// <summary>
    /// Query maker
    /// </summary>
    public class Archivador
    {
        private const string TablaNovelas = "Novelas";
        private const string TablaCapitulos = "Capitulos";
        private const string TablaLinks = "Links";

        /// <summary>
        /// Regresa una instancia de una novela registrada en la base de datos.
        /// </summary>
        /// <param name="infoNov"></param>
        /// <returns></returns>
        public Novela GuardaInfoNovela(InformacionNovela infoNov)
        {
            using (IDbConnection cnn = new SQLiteConnection(DataBaseAccess.GetConnectionString()))
            {
                string obtenIDnovela = $"select ID from {TablaNovelas} where Titulo = '{infoNov.Titulo}'";
                //Revisa si ya existe
                var _ = cnn.Query<int>(obtenIDnovela);

                bool NoExisteLaNovela = _.Any() == false;
                if (NoExisteLaNovela)
                {
                    string queryNovela = $"insert into {TablaNovelas} (Titulo, LinkPrincipal) values" +
                    $"('{infoNov.Titulo}', '{infoNov.LinkPrincipal}')";

                    cnn.Execute(queryNovela);

                    var novelaID = cnn.Query<int>(obtenIDnovela).First();

                    Novela nov = Factory.ObtenNovela(infoNov, novelaID);

                    GuardaLinksDB(nov.LinksDeCapitulos, novelaID);

                    //No se guardan capitulos porque deben estar vacios en este punto.
                    return nov;
                }
                else
                {
                    return ObtenNovela(infoNov);
                }
            }
        }


        #region Metiendo info a la DB

        private void GuardaLinksDB(List<Uri> LinksCapitulos, int NovelaID)
        {
            using (IDbConnection cnn = new SQLiteConnection(DataBaseAccess.GetConnectionString(), true))
            {
                foreach (Uri link in LinksCapitulos)
                {
                    string query = $"insert into {TablaLinks} " +
                                            $"(Link, NovelaID) values" +
                                            $"('{link}', '{NovelaID}')";
                    cnn.Execute(query);
                }
            }
        }


        public void GuardaCapituloDB(Capitulo Capitulo, int novelaID)
        {
            using (IDbConnection cnn = DataBaseAccess.GetConnection())
            {
                //Revisando que el capitulo no exista en la DB.
                string queryRevisando = $"select Numero from {TablaCapitulos} where " +
                $"Numero = '{Capitulo.NumeroCapitulo}' and" +
                $"NovelaID = '{novelaID}'";
                bool existe = cnn.Query<int>(queryRevisando).Any() ? true : false;
                if (existe)
                {
                    throw new Exception("El capitulo ya existe en la base de datos.");
                }



                string query = $"insert into {TablaCapitulos} " +
                                             $"(NovelaID, TextoCapitulo, Numero, Titulo, Link, Valor) values" +
                                             $"('{novelaID}', '{Capitulo.Texto}', '{Capitulo.NumeroCapitulo}', '{Capitulo.TituloCapitulo}', '{Capitulo.Link}', '{Capitulo.Valor}')";
                cnn.Execute(query);
            }

        }

        #endregion


        #region Sacando info de la DB


        private Novela ObtenNovela(InformacionNovela infoNov)
        {
            using(IDbConnection cnn = DataBaseAccess.GetConnection())
            {                
                string qryID = $"select ID from {TablaNovelas} where Titulo = '{infoNov.Titulo}'";
                int NovId = cnn.Query<int>(qryID).First();

                string qryInfo = $"select nv*, links*, caps*" +
                             $"from {TablaNovelas} nv" +
                             $"inner join {TablaLinks} links" +
                             $"inner join {TablaCapitulos} caps" +
                                $"on nv.ID = links.NovelaID = caps.NovelaID";

                Novela Novela = cnn.Query<InformacionNovela, List<string>, List<Capitulo>, Novela>(qryInfo, 
                    (Info, StringsDeLinks, Capitulos) => 
                    {
                        //Info
                        List<Uri> Links = new List<Uri>();
                        StringsDeLinks.ForEach(x => Links.Add(new Uri(x)));
                        Info.LinksDeCapitulos = Links;

                        Novela novela = new Novela(Info, NovId, Capitulos);

                        return novela; 
                    }  
                    ).First();



                throw new NotImplementedException();
            }
        }

        



        #endregion

    }
}
