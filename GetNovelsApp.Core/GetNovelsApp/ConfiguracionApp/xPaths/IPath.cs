using System;
using System.Collections.Generic;

namespace GetNovelsApp.Core.ConfiguracionApp.xPaths
{
    public interface IPath
    {
        Uri Dominio { get; }

        List<string> xPathsLinks { get; }

        List<string> xPathsSiguienteBoton { get; }

        List<string> xPathsTextos { get; }

        List<string> xPathsTitulo { get; }


    }
}