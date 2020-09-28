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

        #region Testing
       

        /// <summary>
        /// Regresa una instancia de una novela registrada en la base de datos.
        /// </summary>
        /// <param name="infoNov"></param>
        /// <returns></returns>
        public NovelaRuntimeModel BuscaNovelaEnDB(Uri LinkNovela)
        {
            using IDbConnection cnn = new SQLiteConnection(DataBaseAccess.GetConnectionString());

            //Revisa si ya existe
            string obtenIDnovela = $"select ID from {TablaNovelas} where LinkPrincipal = '{LinkNovela}'";
            var _ = cnn.Query<int>(obtenIDnovela);

            bool NoExisteLaNovela = _.Any() == false;
            if (NoExisteLaNovela)
            {
                //Obteniendo toda la info de la web y metiendola en la DB:
                NovelaDBModel novDBInfo = CreateNovel(LinkNovela, cnn, out NovelaWebModel infoNov);

                //Capitulos:
                List<Capitulo> CapitulosNovela = CreaCapitulos(infoNov.LinksDeCapitulos);
                GuardaCapitulos(CapitulosNovela, novDBInfo.ID); //Itera los caps y encuentra su info.

                //Regresando una novela para runtime:
                NovelaRuntimeModel nov = new NovelaRuntimeModel(CapitulosNovela, novDBInfo);
                cnn.Dispose();
                return nov;
            }
            else
            {
                cnn.Dispose();
                return SacaNovelaDB(LinkNovela);
            }

        }

        

        /// <summary>
        /// Se llama 2 veces. Cuando empieza el programa (si es que la novela es nueva) y cuando se descarga un cap nuevo.
        /// </summary>
        /// <param name="capitulosVacios"></param>
        /// <param name="novelaID"></param>
        /// <returns></returns>
        public List<Capitulo> GuardaCapitulos(List<Capitulo> capitulosVacios, int novelaID)
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            foreach (Capitulo c in capitulosVacios)
            {
                bool CapExiste = RevisaSiCapExiste_Output(novelaID, cnn, c);

                if (CapExiste) //El cap existe.
                {
                    //Revisa si el capitulo tiene su texto.
                    string query_RevisaCap = $"select TextoCapitulo from {TablaCapitulos} where NovelaID = '{novelaID}' and Link = '{c.Link}'";
                    string output_RevisaCap = cnn.Query<string>(query_RevisaCap).First();

                    if (string.IsNullOrEmpty(output_RevisaCap)) //No tiene texto
                    {
                        // UPDATE table_name SET column_name = value [, column_name = value ...]

                        string query_ActualizaTextoCap =    $"update {TablaCapitulos} " +
                                                            $"set TextoCapitulo = '{c.Texto}' " +
                                                            $"where NovelaID = '{novelaID}' and Link = '{c.Link}'";
                        cnn.Execute(query_ActualizaTextoCap);

                        //Comunicador.ReportaError("Se intentó colocarle un texto a un capitulo que exsite pero que está incompleto. Coloca un query para actualizar la entrada...", this);
                        continue;
                    }
                    
                }
                else //El cap no existe
                {
                    //Toma un capitulo vacio, obtiene su info basica y la mete en la DB.
                    string qry_MeteCapEnDB = InsertCapitulo_Query(novelaID, c);
                    cnn.Execute(qry_MeteCapEnDB);
                }


                
            }


            cnn.Dispose();
            return capitulosVacios; //Ya no estarán vacios tho.
        }



        private NovelaRuntimeModel SacaNovelaDB(Uri LinkNovela) //Debe ir privada.
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            //Encuentra la infobasica
            string qryNovela = $"select * from {TablaNovelas} where LinkPrincipal = '{LinkNovela}'";
            NovelaDBModel infoDBNovela = cnn.Query<NovelaDBModel>(qryNovela).First();

            //Encuentra los capitulos
            string qryCapitlos = $"select Link, TextoCapitulo, Titulo, Numero, Valor from {TablaCapitulos} where NovelaID = '{infoDBNovela.ID}'";
            List<Capitulo> Capitulos = cnn.Query<Capitulo>(qryCapitlos).ToList();

            NovelaRuntimeModel novela = new NovelaRuntimeModel(Capitulos, infoDBNovela);

            cnn.Dispose();
            return novela;
        }

        #endregion


        #region Helpers

        private NovelaDBModel CreateNovel(Uri LinkNovela, IDbConnection cnn, out NovelaWebModel infoNov)
        {
            //Encontrando informacion de la web.
            infoNov = ManipuladorDeLinks.EncuentraInformacionNovela(LinkNovela);

            //Metiendola en la DB.
            string queryNovela = $"insert into {TablaNovelas} (Titulo, LinkPrincipal) values" +
                            $"('{infoNov.Titulo}', '{infoNov.LinkPrincipal}')"; 
            cnn.Execute(queryNovela);

            //Obteniendo un modelo de la DB.
            string query_ObtenDBInfo = $"select * from {TablaNovelas} where LinkPrincipal = '{LinkNovela}'";
            NovelaDBModel novDBInfo = cnn.Query<NovelaDBModel>(query_ObtenDBInfo).First();

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


        /// <summary>
        /// Toma un capitulo vacio, obtiene su info basica y la mete en la DB.
        /// </summary>
        /// <param name="novelaID"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private string InsertCapitulo_Query(int novelaID, Capitulo c)
        {
            string qry_MeteCapEnDB;

            qry_MeteCapEnDB = $"insert into {TablaCapitulos} " +
                                $"(NovelaID, Link, TextoCapitulo, Numero, Titulo, Valor) values" +
                                $"('{novelaID}', '{c.Link}', '{c.Texto}', '{c.NumeroCapitulo}', '{c.TituloCapitulo}', '{c.Valor}')";

            return qry_MeteCapEnDB;
        }




        private List<Capitulo> CreaCapitulos(List<Uri> ListaDeLinks)
        {
            List<Capitulo> Capitulos = new List<Capitulo>();
            foreach (Uri L in ListaDeLinks)
            {
                CapituloWebModel _ = ManipuladorDeLinks.EncuentraInformacionCapitulo(L);
                Capitulo C = new Capitulo(_);
                Capitulos.Add(C);
            }
            return Capitulos;
        }


        #endregion

    }
}
