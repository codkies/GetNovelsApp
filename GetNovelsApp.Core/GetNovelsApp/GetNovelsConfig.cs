using System.Collections.Generic;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.GetNovelsApp;

namespace GetNovelsApp.Core
{
    /// <summary>
    /// Configuracion actual de toda la libreria. Requiere de un IConfig.
    /// </summary>
    public static class GetNovelsConfig
    {
        public static void EstableceConfig(IConfig configuracion)
        {
            ConfiguracionActual = configuracion;
            GetNovelsComunicador.EstableceConfig(configuracion.Comunicador);
            GetNovelsFactory.EstableceConfig(configuracion.Fabrica);
        }

        /// <summary>
        /// Configuracion de la app.
        /// </summary>
        private static IConfig ConfiguracionActual;


        #region Propiedades

        /// <summary>
        /// Tipo de UI que se está usando
        /// </summary>
        public static IFabrica Fabrica => ConfiguracionActual.Fabrica;

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
