using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Conexiones.DB;

namespace GetNovelsApp.Core.ConfiguracionApp.xPaths
{
    /// <summary>
    /// xPaths basicos para un IPath.
    /// </summary>
    public class Website : IPath
    {
        public Website(string dominio, List<string> xpathsLinks, List<string> xpathsTextos, List<string> xpathsTitulo, OrdenLinks OrdenLinks)
        {
            this.dominio = dominio;
            this.xpathsLinks = xpathsLinks;
            this.xpathsTextos = xpathsTextos;
            this.xpathsTitulo = xpathsTitulo;
            ordenLinks = OrdenLinks;
        }

        public Website()
        {
            xpathsLinks = new List<string>();
            xpathsTextos = new List<string>();
            xpathsTitulo = new List<string>();
        }

        public void AgregaLink(string xPathLink)
        {
            xpathsLinks.Add(xPathLink);
        }

        public void AgregaTexto(string xPathTexto)
        {
            xpathsTextos.Add(xPathTexto);
        }

        public void AgregaTitulo(string xPathTitulo)
        {
            xpathsTitulo.Add(xPathTitulo);
        }

        private string dominio;        
        private List<string> xpathsLinks;
        private List<string> xpathsTextos;
        private List<string> xpathsTitulo;
        private OrdenLinks ordenLinks;



        public string Dominio => dominio;

        public List<string> xPathsLinks => xpathsLinks;

        public List<string> xPathsTextos => xpathsTextos;

        public List<string> xPathsTitulo => xpathsTitulo;

        public OrdenLinks OrdenLinks => ordenLinks;
    }
}
