using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;

namespace GetNovelsApp.Core.Conexiones.DB
{
    /// <summary>
    /// Relacionados a manipulacion de Perfiles websites
    /// </summary>
    public partial class Archivador
    {
        private const string TablaPerfilesWebsites = "PerfilesWebsites";
        private const string TablaXPathTextos = "xPathsTextos";
        private const string TablaXPathLinks = "xPathsLinks";
        private const string TablaXPathTitulo = "xPathsTitulo";
        private const string TablaConfiguracion = "Configuracion";
        private const int IDdescendiente = 1;
        private const int IDascendiente = 2;


        public IConfig ObtenConfiguracion()
        {
            string qry = $"Select * from {TablaConfiguracion}";
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            var dynamic = new { CarpetaDescargas = "", CapitulosPorDocumento = 0, TamañoBatch = 25 };
            var resultado = cnn.Query(qry, dynamic).First();


            string CarpetasDescarga = GetProperty(resultado, "CarpetaDescargas") as string;
            int CapitulosPorDocumento = Convert.ToInt32(GetProperty(resultado, "CapitulosPorDocumento"));
            int TamañoBatch = Convert.ToInt32(GetProperty(resultado, "TamañoBatch"));

            return GetNovelsFactory.FabricaConfiguracion(CarpetasDescarga, TamañoBatch, CapitulosPorDocumento);
        }


        public void ActualizaConfiguracion(IConfig config)
        {
            string qry = $"update {TablaConfiguracion} " +
                            $"set CarpetaDescargas = '{config.FolderPath}', " +
                            $"TamañoBatch='{config.TamañoBatch}', " +
                            $"CapitulosPorDocumento='{config.CapitulosPorDocumento}' ";
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            cnn.Execute(qry);

            GetNovelsEvents.Invoke_ConfiguracionCambio();
        }


        public bool PerfilExiste(string dominio)
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            string qry = $"select * from {TablaPerfilesWebsites} where Dominio = '{dominio}'";
            var output = cnn.Query(qry);
            return output.Any();
        }


        public void GuardaPerfil(IPath Website)
        {
            //Tomando referencias.
            string Dominio = Website.Dominio;
            List<string> xPathsTextos = Website.xPathsTextos;
            List<string> xPathsLinks = Website.xPathsLinks;
            List<string> xPathsTitulo = Website.xPathsTitulo;
            int OrdenID = (int)Website.OrdenLinks;

            //Abriendo una conexión a la DB
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            //Metiendo el Dominio
            string qry_InsertDominio = InsertaDominio_Query(Dominio);
            cnn.Execute(qry_InsertDominio);

            //Obteniendo el ID del Dominio
            string qry_ObtenWebsiteID = ObtenWebsiteID_Query(Dominio);
            int WebsiteID = cnn.Query<int>(qry_ObtenWebsiteID).First();

            //Metiendo los xPathsLinks
            foreach (string xPath in xPathsLinks)
            {
                string qry_InsertarXPathsTextos = InsertaXPathLinks_Query(OrdenID, WebsiteID, xPath);
                cnn.Execute(qry_InsertarXPathsTextos);
            }

            //Metiendo xPathsTextos
            foreach (string xPath in xPathsTextos)
            {
                string qry_InsertarXPathsTextos = InsertaXPath_Query(WebsiteID, xPath, TablaXPathTextos);
                cnn.Execute(qry_InsertarXPathsTextos);
            }

            //Metiendo xPathsTitulo
            foreach (string xPath in xPathsTitulo)
            {
                string qry_InsertarXPathsTextos = InsertaXPath_Query(WebsiteID, xPath, TablaXPathTitulo);
                cnn.Execute(qry_InsertarXPathsTextos);
            }

            GetNovelsEvents.Invoke_WebsitesCambiaron();
        }


        public List<IPath> ObtenPerfiles()
        {
            using IDbConnection cnn1 = DataBaseAccess.GetConnection();

            //Obteniendo dominio & ID
            string dominioQry = $"select * from {TablaPerfilesWebsites}";
            var modeloPerfiles = new { WebsiteID = 0, Dominio = "" };
            var ListaPerfiles = cnn1.Query(dominioQry, modeloPerfiles).ToList();

            List<Website> output = new List<Website>();

            foreach (var perfil in ListaPerfiles)
            {
                using IDbConnection cnn = DataBaseAccess.GetConnection();
                string Dominio = GetProperty(perfil, "Dominio");
                int id = Convert.ToInt32(GetProperty(perfil, "WebsiteID"));

                //Obteniendo xPath
                string TituloQry = $"select xPath from {TablaXPathTitulo} where WebsiteID = '{id}'";

                string LinkQry = $"select xPath from {TablaXPathLinks} where WebsiteID = '{id}'";

                string OrdenQry = $"select OrdenID from {TablaXPathLinks} where WebsiteID = '{id}'";

                string TextoQry = $"select xPath from {TablaXPathTextos} where WebsiteID = '{id}'";

                List<string> xPathsTitulo = null;
                List<string> xPathsLinks = null;
                List<string> xPathsTexto = null;
                OrdenLinks orden = 0;

                xPathsTitulo = cnn.Query<string>(TituloQry).ToList();
                xPathsLinks = cnn.Query<string>(LinkQry).ToList();
                xPathsTexto = cnn.Query<string>(TextoQry).ToList();
                orden = cnn.Query<OrdenLinks>(OrdenQry).First();


                Website wb = new Website(Dominio, xPathsLinks, xPathsTexto, xPathsTitulo, orden);
                output.Add(wb);
            }

            return new List<IPath>(output);
        }


        private object GetProperty(object target, string name)
        {
               var site = System.Runtime.CompilerServices.CallSite<Func<System.Runtime.CompilerServices.CallSite, object, object>>.Create(Microsoft.CSharp.RuntimeBinder.Binder.GetMember(0, name, target.GetType(), new[] { Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo.Create(0, null) }));
            return site.Target(site, target);
        }


        #region Queries

        private static string ObtenWebsiteID_Query(string Dominio)
        {

            //agarrando su ID
            return $"SELECT WebsiteID FROM {TablaPerfilesWebsites} " +
                                            $"WHERE Dominio = '{Dominio}' ";
        }

        private static string InsertaDominio_Query(string Dominio)
        {

            //Guardando el Dominio
            return $"INSERT INTO {TablaPerfilesWebsites} " +
                                        $"(Dominio) values " +
                                        $"('{Dominio}')";
        }

        private static string InsertaXPathLinks_Query(int OrdenID, int WebsiteID, string xPath)
        {
            return $"INSERT INTO {TablaXPathLinks} " +
                                                $"(WebsiteID, xPath, OrdenID) values " +
                                                $"('{WebsiteID}', \"{xPath}\", '{OrdenID}')";
        }

        private static string InsertaXPath_Query(int WebsiteID, string xPath, string Tabla)
        {
            return $"INSERT INTO {Tabla} " +
                    $"(WebsiteID, xPath) values " +
                    $"('{WebsiteID}', \"{xPath}\")";
        }

        #endregion


    }

}
