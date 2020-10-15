using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using Dapper;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.Reportaje;

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
            string qry = $"Select * from {i.TConfiguracion}";
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            var dynamic = new { Carpeta = "", CapitulosPorDocumento = 0, TamañoBatch = 25 };
            var resultado = cnn.Query(qry, dynamic).First();


            string CarpetasDescarga = GetProperty(resultado, "Carpeta") as string;
            int CapitulosPorDocumento = Convert.ToInt32(GetProperty(resultado, "CapitulosPorDocumento"));
            int TamañoBatch = Convert.ToInt32(GetProperty(resultado, "TamañoBatch"));

            return GetNovelsFactory.FabricaConfiguracion(CarpetasDescarga, TamañoBatch, CapitulosPorDocumento);
        }

         
        public void ActualizaConfiguracion(IConfig config)
        {
            string qry = $"update {i.TConfiguracion} " +
                            $"set Carpeta = '{config.FolderPath}', " +
                            $"TamañoBatch='{config.TamañoBatch}', " +
                            $"CapitulosPorDocumento='{config.CapitulosPorDocumento}' ";
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            cnn.Execute(qry);

            GetNovelsEvents.Invoke_ConfiguracionCambio();
        } 


        public bool PerfilExiste(string dominio)
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            string qry = $"select * from {i.TWebsites} where Dominio = '{dominio}'";
            var output = cnn.Query(qry);
            return output.Any();
        }

        

        public void GuardaPerfil(IPath Website, IProgress<IReporte> progreso = null)
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


        public bool ActualizaPerfil(IPath perfilNuevo, IPath perfilViejo, IProgress<IReporte> progreso = null)
        {
            //indexGenericoDeActualizacionDePerfil
            int IDprogreso = -10;
            int totalDePasos = 4;
            using IDbConnection cnn = DataBaseAccess.GetConnection();
            if(progreso != null) progreso.Report(new Reporte(totalDePasos, 0, "Actualizando perfil", IDprogreso, this, perfilViejo.Dominio));

            //Hayando el ID del Perfil en la DB
            string encuentraPerfilViejo = $"select WebsiteID from {i.TWebsites} WHERE Dominio = '{perfilViejo.Dominio}'";
            int websiteID = cnn.Query<int>(encuentraPerfilViejo).First();
            if(progreso != null) progreso.Report(new Reporte(totalDePasos, 1, "Actualizando perfil", IDprogreso, this, perfilViejo.Dominio));

            //Condiciones
            bool linksCambiaron = perfilNuevo.xPathsLinks.Except(perfilViejo.xPathsLinks).Any() | perfilViejo.xPathsLinks.Except(perfilNuevo.xPathsLinks).Any();
            bool ordenCambio = perfilNuevo.OrdenLinks != perfilViejo.OrdenLinks;
            bool textosCambiaron = perfilNuevo.xPathsTextos.Except(perfilViejo.xPathsTextos).Any() | perfilViejo.xPathsTextos.Except(perfilNuevo.xPathsTextos).Any();
            bool titulosCambiaron = perfilNuevo.xPathsTitulo.Except(perfilViejo.xPathsTitulo).Any() | perfilViejo.xPathsTitulo.Except(perfilNuevo.xPathsTitulo).Any();

            //Tabla de Links
            if (linksCambiaron)
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    //Borrando los viejos
                    string updateLinks = $"DELETE from {i.TxPathsLinks} WHERE WebsiteID = '{websiteID}' ";
                    cnn.Execute(updateLinks);

                    //Metiendo los nuevos
                    foreach (string xpath in perfilNuevo.xPathsLinks)
                    {
                        string query = $"insert into {i.TxPathsLinks} (WebsiteID, xPath, OrdenID) " +
                                        $"values ('{websiteID}', '{xpath}', '{(int)perfilNuevo.OrdenLinks}') ";
                        try
                        {
                            cnn.Execute(query);
                        }
                        catch (Exception ex)
                        {
                            GetNovelsComunicador.ReportaErrorMayor($"No se pudo actualizar el perfil {perfilViejo.Dominio}\n" +
                                $"Error: {ex.Message}", this);
                            return false;
                        }

                    }
                    //Finalizando transaccion
                    transaction.Commit();
                }
            }
            else if (ordenCambio)
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    //Metiendo los nuevos
                    foreach (string xpath in perfilNuevo.xPathsLinks)
                    {
                        string query = $"UPDATE {i.TxPathsLinks} set OrdenID = '{(int)perfilNuevo.OrdenLinks}') " +
                                        $"WHERE xPath = '{xpath}'";
                        try
                        {
                            cnn.Execute(query);
                        }
                        catch (Exception ex)
                        {
                            GetNovelsComunicador.ReportaErrorMayor($"No se pudo actualizar el perfil {perfilViejo.Dominio}\n" +
                                $"Error: {ex.Message}", this);
                            return false;
                        }

                    }
                    //Finalizando transaccion
                    transaction.Commit();
                }
            }
            if (progreso != null) progreso.Report(new Reporte(totalDePasos, 2, "Actualizando perfil", IDprogreso, this, perfilViejo.Dominio));

            //Tabla de textos
            if (textosCambiaron)
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    //Borrando los viejos
                    string updateLinks = $"DELETE from {i.TxPathsTextos} WHERE WebsiteID = '{websiteID}' ";
                    cnn.Execute(updateLinks);

                    //Metiendo los nuevos
                    foreach (string xpath in perfilNuevo.xPathsTextos)
                    {
                        string query = $"insert into {i.TxPathsTextos} (WebsiteID, xPath) " +
                                        $"values ('{websiteID}', '{xpath}')";
                        try
                        {
                            cnn.Execute(query);
                        }
                        catch (Exception ex)
                        {
                            GetNovelsComunicador.ReportaErrorMayor($"No se pudo actualizar el perfil {perfilViejo.Dominio}\n" +
                                $"Error: {ex.Message}", this);
                            return false;
                        }

                    }
                    //Finalizando transaccion
                    transaction.Commit();
                }
            }
            if (progreso != null) progreso.Report(new Reporte(totalDePasos, 3, "Actualizando perfil", -IDprogreso, this, perfilViejo.Dominio));

            //Tabla de Titulos
            if (titulosCambiaron)
            {
                cnn.Open();
                using (var transaction = cnn.BeginTransaction())
                {
                    //Borrando los viejos
                    string updateLinks = $"DELETE from {i.TxPathsTitulo} WHERE WebsiteID = '{websiteID}' ";
                    cnn.Execute(updateLinks);

                    //Metiendo los nuevos
                    foreach (string xpath in perfilNuevo.xPathsTitulo)
                    {
                        string query = $"insert into {i.TxPathsTitulo} (WebsiteID, xPath) " +
                                        $"values ('{websiteID}', \"{xpath}\")";
                        try
                        {
                            cnn.Execute(query);
                        }
                        catch (Exception ex)
                        {
                            GetNovelsComunicador.ReportaErrorMayor($"No se pudo actualizar el perfil {perfilViejo.Dominio}\n" +
                                $"Error: {ex.Message}", this);
                            return false;
                        }

                    }
                    //Finalizando transaccion
                    transaction.Commit();
                }
            }
            if (progreso != null) progreso.Report(new Reporte(totalDePasos, 4, "Actualizando perfil", IDprogreso, this, perfilViejo.Dominio));

            GetNovelsEvents.Invoke_WebsitesCambiaron();
            return true;
        }


        public List<IPath> ObtenPerfiles()
        {
            using IDbConnection cnn = DataBaseAccess.GetConnection();

            //Obteniendo dominio & ID
            string dominioQry = $"select * from {i.TWebsites}";
            var modeloPerfiles = new { WebsiteID = 0, Dominio = "" };
            var ListaPerfiles = cnn.Query(dominioQry, modeloPerfiles).ToList();

            List<Website> output = new List<Website>();

            foreach (var perfil in ListaPerfiles)
            {
                string Dominio = GetProperty(perfil, "Dominio");
                int id = Convert.ToInt32(GetProperty(perfil, "WebsiteID"));

                //Obteniendo xPath
                string TituloQry = $"select xPath from {i.TxPathsTitulo} where WebsiteID = '{id}'";

                string LinkQry = $"select xPath from {i.TxPathsLinks} where WebsiteID = '{id}'";

                string OrdenQry = $"select OrdenID from {i.TxPathsLinks} where WebsiteID = '{id}'";

                string TextoQry = $"select xPath from {i.TxPathsTextos} where WebsiteID = '{id}'";

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
            return $"SELECT WebsiteID FROM {i.TWebsites} " +
                                            $"WHERE Dominio = '{Dominio}' ";
        }

        private static string InsertaDominio_Query(string Dominio)
        {
            return $"INSERT INTO {i.TWebsites} " +
                                        $"(Dominio) values " +
                                        $"('{Dominio}')";
        }

        private static string InsertaXPathLinks_Query(int OrdenID, int WebsiteID, string xPath)
        {
            return $"INSERT INTO {i.TxPathsLinks} " +
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
