using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.Empaquetador
{
    /// <summary>
    /// Define el path donde se guarda una novela.
    /// </summary>
    public static class LocalPathManager
    {
        public static string DefinePathNovela(INovela novela, bool Subdirectorio = false)
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
    }
}
