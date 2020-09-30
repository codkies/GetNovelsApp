using System.Collections.Generic;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.Core.ConfiguracionApp
{
    /// <summary>
    /// Shorthand para una configuracion basica generica.
    /// </summary>
    public abstract class ConfiguracionBasica : IConfig
    { 
        #region Contractor privado

        protected abstract int capsPorDocumento { get; }

        protected abstract int tamañoBatch { get; }

        protected abstract string direccionDiscoDuro { get; }

        protected abstract IPath xPaths { get; }

        protected abstract IComunicador comunicador { get; }

        protected abstract IFabrica fabrica { get; }

        #endregion

        #region Contracto publico (IConfig)


        public int TamañoBatch => tamañoBatch;

        public int CapitulosPorDocumento => capsPorDocumento;

        public List<string> xPathsTextos => xPaths.xPathsTextos;

        public List<string> xPathsSiguienteBoton => xPaths.xPathsSiguienteBoton;

        public List<string> xPathsTitulo => xPaths.xPathsTitulo;

        public List<string> xPathsLinks => xPaths.xPathsLinks;

        public IComunicador Comunicador => comunicador;

        public string FolderPath => direccionDiscoDuro;

        public IFabrica Fabrica => fabrica;

        public List<string> xPathsSipnosis => xPaths.xPathsSipnosis;

        public List<string> xPathsImagen => xPaths.xPathsImagen;

        public List<string> xPathsTags => xPaths.xPathsTags;
        #endregion
    }
}
