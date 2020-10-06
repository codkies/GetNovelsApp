﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Utilidades;

namespace GetNovelsApp.Core.Conexiones.DB
{
    /// <summary>
    /// Query maker
    /// </summary>
    public partial class Archivador : IReportero
    {
        #region fields
        private const string TablaNovelas = "Novelas";
        private const string TablaCapitulos = "Capitulos";
        private const string TablaClasificacion = "Clasificacion";
        private const string TablaAutor = "Autores";

        /// <summary>
        /// Define si se están guardando capitulos
        /// </summary>
        private bool EjecutandoGuardado = false;

        public string Nombre => "DB"; 
        #endregion


        #region Core


        /// <summary>
        /// Toma una informacion de novela online y la guarda.
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public async Task<INovela> MeteNovelaDBAsync(InformacionNovelaOnline info)
        {
            bool YaExiste = NovelaExisteEnDB(info.LinkPrincipal);
            if (!YaExiste)
            {
                using IDbConnection cnn = DataBaseAccess.GetConnection();
                //Obteniendo toda la info de la web y metiendola en la DB:
                InformacionNovelaDB novDBInfo = InsertaNovelaEnDB(info, cnn);

                //Capitulos:
                List<Capitulo> CapitulosNovela = GetNovelsFactory.FabricaCapitulos(info.LinksDeCapitulos);
                await GuardaCapitulosAsync(CapitulosNovela, novDBInfo.ID); //Itera los caps y encuentra su info.

                //Regresando una novela para runtime:
                INovela nov = GetNovelsFactory.FabricaNovela(CapitulosNovela, novDBInfo);
                cnn.Dispose();
                return nov;
            }
            else
            {
                return SacaNovelaDB(info.LinkPrincipal);
            }
        }

        Dictionary<Capitulo, int> CapitulosAGuardar = new Dictionary<Capitulo, int>();

        public async Task GuardaCapitulosAsync(Capitulo capitulo, int novelaID)
        {
            CapitulosAGuardar.Add(capitulo, novelaID);
            if (EjecutandoGuardado) return;
            await Task.Run(() => CapitulosAGuardar_Ejecuta());
        }

        public async Task GuardaCapitulosAsync(List<Capitulo> capitulos, int novelaID)
        {
            capitulos.ForEach(x => CapitulosAGuardar.Add(x, novelaID));            
            if (EjecutandoGuardado) return;
            await Task.Run(() => CapitulosAGuardar_Ejecuta());
        }


        /// <summary>
        /// Regresa true or false dependiendo si el link ingresado, corresponde a alguna novela en la DB.
        /// </summary>
        /// <param name="Link"></param>
        /// <returns></returns>
        public bool NovelaExisteEnDB(Uri Link)
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            string qry = GetIDNovel_Query(Link);
            var resultados = cnn.Query<int>(qry);
            cnn.Dispose();

            return resultados.Any();
        }



        public async Task<List<INovela>> ObtenTodasNovelasAsync()
        {
            List<INovela> output = new List<INovela>();
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            //Obteniendo las informaciones de novela:
            string getThemAll = GetAllNovels_Query();
            List<InformacionNovelaDB> InfosDeNovelas = await Task.Run(() => cnn.Query<InformacionNovelaDB>(getThemAll).ToList());

            //Obteniendo los capitulos de todas:
            List<Task> tareas = new List<Task>();
            foreach (var InfoNov in InfosDeNovelas)
            {
                var tarea = Task.Run(() => MeteCapitulosEnINovela(output, InfoNov));
                tareas.Add(tarea);
            }

            await Task.WhenAll(tareas);


            return output;
        }


        #endregion


        #region Queries

        /// <summary>
        /// Query para obtener info de una novela en la tabla de novelas.
        /// </summary>
        /// <param name="infoDBNovela"></param>
        /// <returns></returns>
        private static string GetChaptersOfNovel_Query(InformacionNovelaDB infoDBNovela)
        {
            return $"select Link, TextoCapitulo, Titulo, Numero, Valor from {TablaCapitulos} where NovelaID = '{infoDBNovela.ID}'";
        }


        /// <summary>
        /// Query para obtener una InformacionNovelaDB según el ID de la novela.
        /// </summary>
        /// <param name="novelID"></param>
        /// <returns></returns>
        private static string GetNovDBInfoWithID_Query(int novelID)
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
        private string GetAllNovels_Query()
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
        private string GetIDNovel_Query(Uri LinkNovela)
        {
            return $"select ID from {TablaNovelas} where LinkPrincipal = '{LinkNovela}'";
        }


        /// <summary>
        /// Query para insertar una novela en la DB
        /// </summary>
        /// <param name="infoNov"></param>
        /// <returns></returns>
        private string InsertNovel_Query(InformacionNovelaOnline infoNov)
        {
            return $"insert into {TablaNovelas} " +
                    $"(Titulo, LinkPrincipal, Sipnosis, Imagen) values" +
                    $"('{infoNov.Titulo}', '{infoNov.LinkPrincipal}', '{infoNov.Sipnosis}', '{infoNov.Imagen}')";
        }


        /// <summary>
        /// Query para insertar los tags de una novela en la DB.
        /// </summary>
        /// <param name="infoNov"></param>
        /// <param name="ID"></param>
        /// <returns></returns>
        private string InsertTags_Query(InformacionNovelaOnline infoNov, int ID)
        {
            return $"insert into {TablaClasificacion} " +
                    $"(NovelaID, Tags) values" +
                    $"('{ID}', '{ManipuladorStrings.TagsEnString(infoNov.Tags)}')";
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


        /// <summary>
        /// Query para actualizar un capitulo en la DB.
        /// </summary>
        /// <param name="novelaID"></param>
        /// <param name="c"></param>
        /// <returns></returns>
        private string UpdateCapitulo_Query(int novelaID, Capitulo c)
        {
            return  $"update {TablaCapitulos} " +
                    $"set TextoCapitulo = \"{c.Texto}\" " +
                    $"where NovelaID = '{novelaID}' and Link = '{c.Link}'";
        }

        #endregion


        #region Helpers

        /// <summary>
        /// Usando el Diccionario de CapitulosAGuardar, este metodo ejecutará queries a la DB mientras queden capitulos en el dicc. (Cada vez que guarda 1, lo remueve del dicc).
        /// </summary>
        private void CapitulosAGuardar_Ejecuta()
        {
            if (CapitulosAGuardar.Count < 1) return;
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            EjecutandoGuardado = true;
            while (CapitulosAGuardar.Count > 0) //Mientras hayan capitulos por guardar...
            {
                var key = CapitulosAGuardar.First();
                int NovelaID = key.Value;
                Capitulo c = key.Key;

                try
                {
                    var qry = InsertCapitulo_Query(NovelaID, c);
                    cnn.Execute(qry);
                }
                catch (SQLiteException)
                {
                    var updateQry = UpdateCapitulo_Query(NovelaID, c);
                    cnn.Execute(updateQry);

                }
                catch (Exception ex)
                {
                    GetNovelsComunicador.ReportaError($"Error: {ex.Message}", this);
                }

                CapitulosAGuardar.Remove(c);
            }
            EjecutandoGuardado = false;
            cnn.Dispose();
        }

        /// <summary>
        /// Mete los capitulos de una novela en un INovela y mete la dicha INovela en el Locker.
        /// </summary>
        /// <param name="locker"></param>
        /// <param name="InfoNov"></param>
        private void MeteCapitulosEnINovela(List<INovela> locker, InformacionNovelaDB InfoNov)
        {
            using (IDbConnection cnn = DataBaseAccess.GetConnection())
            {
                string getThemChapters = GetChaptersOfNovel_Query(InfoNov);
                var Capitulos = cnn.Query<Capitulo>(getThemChapters);
                locker.Add(GetNovelsFactory.FabricaNovela(Capitulos, InfoNov));
            }
        }


        private INovela SacaNovelaDB(Uri LinkNovela) //Debe ir privada.
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            //Encuentra la infobasica
            string qryNovela = GetIDNovel_Query(LinkNovela);
            InformacionNovelaDB infoDBNovela = cnn.Query<InformacionNovelaDB>(qryNovela).First();

            //Encuentra los capitulos
            string qryCapitlos = GetChaptersOfNovel_Query(infoDBNovela);
            List<Capitulo> Capitulos = cnn.Query<Capitulo>(qryCapitlos).ToList();
            
            //Construye la Inovela
            INovela novela = GetNovelsFactory.FabricaNovela(Capitulos, infoDBNovela);

            cnn.Dispose();
            return novela;
        }
        

        private InformacionNovelaDB InsertaNovelaEnDB(InformacionNovelaOnline infoNov, IDbConnection cnn)
        {
            //Insertando la novela
            string qry = InsertNovel_Query(infoNov);
            cnn.Execute(qry);

            //Obteniendo el ID
            string qID = $"select ID from {TablaNovelas} where LinkPrincipal = \"{infoNov.LinkPrincipal}\" ";
            int novelID = cnn.Query<int>(qID).First();

            //Insertando las tags
            string qry_tags = InsertTags_Query(infoNov, novelID);
            cnn.Execute(qry_tags);

            //Obteniendo una referencia de la DB
            string query_ObtenDBInfo = GetNovDBInfoWithID_Query(novelID);

            InformacionNovelaDB novDBInfo = cnn.Query<InformacionNovelaDB>(query_ObtenDBInfo).First();

            GetNovelsEvents.Invoke_NovelaAgregadaADB();
            return novDBInfo;
        }


        #endregion
    }






    internal static class Insertador
    {
        //enumsish
        const string TEstadoHistoria = "EstadoHistoria";
        const string TEstadoTraduccion = "EstadoTraduccion";
        const string TNaciones = "Naciones";
        const string TOrdenLinks = "OrdenLinks";
        const string TTags = "Tags";
        const string TGeneros = "Generos";

        //novelas
        const string TAutores = "Autores";
        const string TEstadoNovela = "EstadoNovelas";
        const string TLinks = "Links";
        const string TImagenes = "Imagenes";
        const string TNovelas = "Novelas";
        const string TReviews = "ReviewsNovelas";
        const string TSipnosis = "Sipnosis";
        const string TGenerosNovela = "GenerosNovela";
        const string TTagsNovelas = "TagsNovelas";

        const string TCapitulos = "Capitulos";
        const string TTextosCapitulos = "TextosCapitulos";

        //configs
        const string TConfiguracion = "Configuracion";
        const string TWebsites = "Websites";
        const string TxPathsLinks = "xPathsLinks";

        const string TxPathsTextos = "xPathsTextos";
        const string TxPathsTitulo = "xPathsTitulo";


    }

}
