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
        private static Archivador ar;
        public static void InicializaConfig()
        {
            ar = new Archivador();
            ActualizaWebsites();
            ActualizaConfiguracion();
            GetNovelsEvents.WebsitesCambiaron += ActualizaWebsites;
            GetNovelsEvents.ConfiguracionCambio += ActualizaConfiguracion;
        }

        private static void ActualizaConfiguracion()
        {
            ConfiguracionActual = ar.ObtenConfiguracion();
        }

        private static void ActualizaWebsites()
        {
            WebsitesSoportados = ar.ObtenPerfiles();
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


       

        public static List<string> xPathsSipnosis => sipnosis;
        public static List<string> xPathsImagen => imagen;
        public static List<string> xPathsTags => tags;
        public static List<string> xPathsGeneros => generos;  
        public static List<string> xPathsReview => review;
        public static List<string> xPathsEstadoHistoria => estadoHistoria;
        public static List<string> xPathsEstadoTraduccion => estadoTraduccion;
        public static List<string> xPathsNacionalidad => nacionalidad;
        public static List<string> xPathsAutor => autor;
        public static List<string> xPathsNU => novelUpdates;


        private static readonly List<string> novelUpdates = new List<string>()
        {
            "//div[@class='search_title']/a"
        };


        private static readonly List<string> review = new List<string>()
        {
            "//span[@class='uvotes']"
        };
        private static readonly List<string> estadoHistoria = new List<string>()
        {
            "//div[@id='editstatus']"
        };
        private static readonly List<string> estadoTraduccion = new List<string>()
        {
            "//div[@id='showtranslated']/a"
        };
        private static readonly List<string> nacionalidad = new List<string>()
        {
            "//div[@id='showlang']/a"
        };
        private static readonly List<string> autor = new List<string>()
        {
            "//div[@id='showauthors']/a"
        };
        private static readonly List<string> generos = new List<string>()
        {
            "//div[@id='seriesgenre']/a"
        };
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




        #endregion

        #endregion

    }
}
