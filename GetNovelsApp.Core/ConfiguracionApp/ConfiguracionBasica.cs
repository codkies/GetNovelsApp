using System.Collections.Generic;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.Core.ConfiguracionApp
{
    /// <summary>
    /// Shorthand para una configuracion basica generica.
    /// </summary>
    public class ConfiguracionBasica : IConfig
    {
        public ConfiguracionBasica(int tamañoBatch, int capitulosPorDocumento, string folderPath)
        {
            TamañoBatch = tamañoBatch;
            CapitulosPorDocumento = capitulosPorDocumento;
            FolderPath = folderPath;
        }


        #region Contracto publico (IConfig)


        public int TamañoBatch { get; private set; }

        public int CapitulosPorDocumento { get; private set; }

        public string FolderPath { get; private set; }


        #endregion
    }
}
