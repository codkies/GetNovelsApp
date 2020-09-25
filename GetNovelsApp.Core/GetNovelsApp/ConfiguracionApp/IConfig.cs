using System.Collections.Generic;

namespace GetNovelsApp.Core.ConfiguracionApp
{
    /// <summary>
    /// Hacerla una clase?
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Cantidad de capitulos que se intentan conseguir a la vez.
        /// </summary>
        int TamañoBatch { get; }

        /// <summary>
        /// Capitulos por pdf.
        /// </summary>
        int CapitulosPorPdf { get; }

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
