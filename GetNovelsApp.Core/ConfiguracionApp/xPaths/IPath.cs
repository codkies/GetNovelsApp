using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Conexiones.DB;

namespace GetNovelsApp.Core.ConfiguracionApp.xPaths
{



    public interface IPath
    {
        /// <summary>
        /// Orden en que se leen los links cuando se lee desde la página web.
        /// </summary>
        OrdenLinks OrdenLinks { get; }

        string Dominio { get; }

        List<string> xPathsLinks { get; }

        List<string> xPathsTextos { get; }

        List<string> xPathsTitulo { get; }

    }
}