using System;
using System.Collections.Generic;
using System.Linq;

namespace GetNovelsApp.Core.Modelos
{
    public struct InformacionNovela
    {
        public InformacionNovela(string titulo, Uri linkPaginaPrincipal, List<Uri> linksDeCapitulos)
        {
            Titulo = titulo;
            LinkPrincipal = linkPaginaPrincipal;
            LinksDeCapitulos = linksDeCapitulos;
        }


        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo;

        /// <summary>
        /// Link a su pagina principal de la novela
        /// </summary>
        public Uri LinkPrincipal;


        /// <summary>
        /// Lista de todos los links de los capitulos
        /// </summary>
        public List<Uri> LinksDeCapitulos;
    }
}
