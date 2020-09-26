using System;
using System.Collections.Generic;

namespace GetNovelsApp.Core.ConfiguracionApp.xPaths
{
    /// <summary>
    /// xPaths basicos para un IPath.
    /// </summary>
    public class Website : IPath
    {
        public Website(Uri dominio, List<string> xpathsLinks, List<string> xpathsSiguienteBoton, List<string> xpathsTextos, List<string> xpathsTitulo)
        {
            this.dominio = dominio;
            this.xpathsLinks = xpathsLinks;
            this.xpathsSiguienteBoton = xpathsSiguienteBoton;
            this.xpathsTextos = xpathsTextos;
            this.xpathsTitulo = xpathsTitulo;
        }

        private readonly Uri dominio;
        
        private readonly List<string> xpathsLinks;
        private readonly List<string> xpathsSiguienteBoton;
        private readonly List<string> xpathsTextos;
        private readonly List<string> xpathsTitulo;


        public Uri Dominio => dominio;

        public List<string> xPathsLinks => xpathsLinks;

        public List<string> xPathsSiguienteBoton => xpathsSiguienteBoton;

        public List<string> xPathsTextos => xpathsTextos;

        public List<string> xPathsTitulo => xpathsTitulo;
    }
}
