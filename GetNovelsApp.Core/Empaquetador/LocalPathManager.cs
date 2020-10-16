using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.Empaquetador
{
    /// <summary>
    /// Define el path donde se guarda una novela.
    /// </summary>
    public static class LocalPathManager
    {
        public static string DefinePathNovela(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, bool Subdirectorio = true)
        {
            if (Subdirectorio)
            {
                return $"{GetNovelsConfig.HardDrivePath}\\";
            }
            else
            {
                return $"{GetNovelsConfig.HardDrivePath}\\{novela.Titulo}\\";
            }
        }

        public static string DefinePathNovela(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, int primerCap, int ultimoCap,
            bool Subdirectorio = true)
        {
            if (Subdirectorio)
            {
                return $"{GetNovelsConfig.HardDrivePath}\\{novela.Titulo} - {primerCap}-{ultimoCap}.pdf";
            }
            else
            {
                return $"{GetNovelsConfig.HardDrivePath}\\{novela.Titulo}\\";
            }
        }

    }
}
