using System.Collections.Generic;
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

        IComunicador Comunicador { get; }

        /// <summary>
        /// Cantidad de capitulos que se intentan conseguir a la vez.
        /// </summary>
        int TamañoBatch { get; }

        /// <summary>
        /// Capitulos por pdf.
        /// </summary>
        int CapitulosPorDocumento { get; }

        /// <summary>
        /// Lista de xPaths a revisar.
        /// </summary>
        List<string> xPathsTextos { get; }

        /// <summary>
        /// Lista de xPaths para conseguir los botones next.
        /// </summary>
        List<string> xPathsSiguienteBoton { get; }

        /// <summary>
        /// Lista de xPaths para conseguir los titulos.
        /// </summary>
        List<string> xPathsTitulo { get; }

        /// <summary>
        /// Lista de xPaths para conseguir los links de los capitulos.
        /// </summary>
        List<string> xPathsLinks { get; }

    }
}
