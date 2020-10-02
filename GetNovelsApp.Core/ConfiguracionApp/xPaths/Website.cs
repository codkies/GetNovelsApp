using System;
using System.Collections.Generic;

namespace GetNovelsApp.Core.ConfiguracionApp.xPaths
{
    /// <summary>
    /// xPaths basicos para un IPath.
    /// </summary>
    public class Website : IPath
    {
        public Website(Uri dominio, List<string> xpathsLinks, List<string> xpathsSiguienteBoton, List<string> xpathsTextos, List<string> xpathsTitulo, List<string> xpathsSipnosis, List<string> xPathsimagen, List<string> xpathsTags)
        {
            this.dominio = dominio;
            this.xpathsLinks = xpathsLinks;
            this.xpathsSiguienteBoton = xpathsSiguienteBoton;
            this.xpathsTextos = xpathsTextos;
            this.xpathsTitulo = xpathsTitulo;
            this.xpathsSipnosis = xpathsSipnosis;
            this.xPathsimagen = xPathsimagen;
            this.xpathsTags = xpathsTags;
        }


        private readonly Uri dominio;        
        private readonly List<string> xpathsLinks;
        private readonly List<string> xpathsSiguienteBoton;
        private readonly List<string> xpathsTextos;
        private readonly List<string> xpathsTitulo;
        private readonly List<string> xpathsSipnosis;
        private readonly List<string> xPathsimagen;
        private readonly List<string> xpathsTags;
        

        public Uri Dominio => dominio;

        public List<string> xPathsLinks => xpathsLinks;

        public List<string> xPathsSiguienteBoton => xpathsSiguienteBoton;

        public List<string> xPathsTextos => xpathsTextos;

        public List<string> xPathsTitulo => xpathsTitulo;

        public List<string> xPathsSipnosis => xpathsSipnosis;

        public List<string> xPathsImagen => xPathsimagen;

        public List<string> xPathsTags => xpathsTags;
    }
}
