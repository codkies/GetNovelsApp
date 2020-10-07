using System;
using System.Collections.Generic;

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

        public string Autor;

        public string Nacionalidad;

        public bool HistoriaCompletada;

        public bool TraduccionCompletada;

        public float Review;

        public int CantidadReviews;

        public List<string> Generos;

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

       
    }
}
