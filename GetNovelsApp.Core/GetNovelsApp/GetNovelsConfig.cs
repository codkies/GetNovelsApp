using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.GetNovelsApp;
using Org.BouncyCastle.Asn1;

namespace GetNovelsApp.Core
{
    /// <summary>
    /// Configuracion actual de toda la libreria. Requiere de un IConfig.
    /// </summary>
    public static class GetNovelsConfig
    {
        public static void InicializaConfig()
        {
            ActualizaWebsites();
            ActualizaConfiguracion();
            GetNovelsEvents.WebsitesCambiaron += ActualizaWebsites;
            GetNovelsEvents.ConfiguracionCambio += ActualizaConfiguracion;
        }

        private static void ActualizaConfiguracion()
        {
            ConfiguracionActual = new Archivador().ObtenConfiguracion();
        }

        private static void ActualizaWebsites()
        {
            WebsitesSoportados = new Archivador().ObtenPerfiles();
        }


        #region Propiedades


        //Instancias de interfaces

        /// <summary>
        /// Configuracion de la app.
        /// </summary>
        private static IConfig ConfiguracionActual;


        static List<IPath> WebsitesSoportados = new List<IPath>();

        public static List<string> xPathsDeTextos(string dominio)
        {
            foreach (IPath websites in WebsitesSoportados)
            {
                if (websites.Dominio == dominio) return websites.xPathsTextos;
            }
            return null;
        }


        public static List<string> xPathsDeLinks(string dominio)
        {
            foreach (IPath websites in WebsitesSoportados)
            {
                if (websites.Dominio == dominio) return websites.xPathsLinks;
            }
            return null;
        }


        public static List<string> xPathsDeTitulo(string dominio)
        {
            foreach (IPath websites in WebsitesSoportados)
            {
                if (websites.Dominio == dominio) return websites.xPathsTitulo;
            }
            return null;
        }

        public static OrdenLinks OrdenWebsite(string dominio)
        {
            foreach (IPath websites in WebsitesSoportados)
            {
                if (websites.Dominio == dominio) return websites.OrdenLinks;
            }
            return OrdenLinks.NULL;
        }



        //Props para el resto
        /// <summary>
        /// Direccion en el disco duro de la carpeta donde el usuario quiere que se guarden las novelas.
        /// </summary>
        public static string HardDrivePath => ConfiguracionActual.FolderPath;

        /// <summary>
        /// Cantidad de capitulos que se intentan conseguir a la vez.
        /// </summary>
        public static int TamañoBatch => ConfiguracionActual.TamañoBatch;

        /// <summary>
        /// Capitulos por pdf.
        /// </summary>
        public static int CapitulosPorPdf => ConfiguracionActual.CapitulosPorDocumento;
      


        #region Cambia esto luego (NU)


        static List<string> sipnosis = new List<string>()
            {
                "//div[@id='editdescription']/p"
            };


        static List<string> imagen = new List<string>()
            {
                "//div[@class='seriesimg']/img"
            };


        static List<string> tags = new List<string>()
            {
                "//div[@id='showtags']/a"
            };



        public static List<string> xPathsSipnosis => sipnosis;

        public static List<string> xPathsImagen => imagen;

        public static List<string> xPathsTags => tags;

        #endregion

        #endregion

    }
}
