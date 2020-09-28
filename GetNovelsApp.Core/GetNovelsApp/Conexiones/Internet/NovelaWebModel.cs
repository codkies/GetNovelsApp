using System;
using System.Collections.Generic;

namespace GetNovelsApp.Core.Conexiones.Internet
{
    /// <summary>
    /// Modelo que utiliza la DB para obtener info acerca de una novela online.
    /// </summary>
    public struct NovelaWebModel
    {
        public NovelaWebModel(string titulo, Uri linkPaginaPrincipal, List<Uri> linksDeCapitulos)
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
