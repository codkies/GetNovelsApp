using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;

namespace GetNovelsApp.Core.Conexiones.Internet
{
    /// <summary>
    /// Modelo que utiliza la DB para obtener info acerca de una novela online.
    /// </summary>
    public class InformacionNovelaOnline
    {
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


        public Uri Imagen;


        public string Sipnosis;


        public List<string> Tags;


        public InformacionNovelaOnline(string titulo, Uri linkPrincipal, List<Uri> linksDeCapitulos, Uri imagen, string sipnosis, List<string> tags)
        {
            Titulo = titulo;
            LinkPrincipal = linkPrincipal;
            LinksDeCapitulos = linksDeCapitulos;
            Imagen = imagen;
            Sipnosis = sipnosis;
            Tags = tags;            
        }
    }
}
