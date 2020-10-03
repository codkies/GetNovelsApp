using System.Collections.Generic;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.Core.ConfiguracionApp
{

    /// <summary>
    /// Interfaz para la configuracion de la app.
    /// </summary>
    public interface IConfig
    {

        /// <summary>
        /// Direccion en el disco duro donde se guardarán las novelas.
        /// </summary>
        string FolderPath { get; }

        /// <summary>
        /// Cantidad de capitulos que se intentan conseguir a la vez.
        /// </summary>
        int TamañoBatch { get; }

        /// <summary>
        /// Capitulos por pdf.
        /// </summary>
        int CapitulosPorDocumento { get; }
    }
}
