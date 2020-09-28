using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.Empaquetador
{
    /// <summary>
    /// Define el path donde se guarda una novela.
    /// </summary>
    public static class LocalPathManager
    {
        public static string DefinePathNovela(NovelaRuntimeModel novela)
        {
            return $"{GetNovelsConfig.HardDrivePath}\\{novela.Titulo}\\";
        }
    }
}
