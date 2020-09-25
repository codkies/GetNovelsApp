using System.Collections.Generic;
using GetNovelsApp.Core.ConfiguracionApp;

namespace GetNovelsApp.Core.Configuracion
{
    /// <summary>
    /// Configuracion actual de toda la libreria.
    /// </summary>
    public static class AppGlobalConfig 
    {
        public static void EstableceConfig(IConfig configuracion)
        {
            ConfiguracionActual = configuracion;
        }

        /// <summary>
        /// Configuracion de la app.
        /// </summary>
        private static IConfig ConfiguracionActual;


        #region Propiedades

        /// <summary>
        /// Cantidad de capitulos que se intentan conseguir a la vez.
        /// </summary>
        public static int TamañoBatch => ConfiguracionActual.TamañoBatch;

        /// <summary>
        /// Capitulos por pdf.
        /// </summary>
        public static int CapitulosPorPdf => ConfiguracionActual.CapitulosPorPdf;


        /// <summary>
        /// Lista de xPaths a revisar.
        /// </summary>
        public static List<string> xPathsTextos => ConfiguracionActual.xPathsTextos;


        /// <summary>
        /// Lista de xPaths para conseguir los botones next.
        /// </summary>
        public static List<string> xPathsSiguienteBoton => ConfiguracionActual.xPathsSiguienteBoton;

        /// <summary>
        /// Lista de xPaths para conseguir los titulos.
        /// </summary>
        public static List<string> xPathsTitulo => ConfiguracionActual.xPathsTitulo;


        /// <summary>
        /// Lista de xPaths para conseguir los links de los capitulos.
        /// </summary>
        public static List<string> xPathsLinks => ConfiguracionActual.xPathsLinks;

        #endregion

    }   
}
