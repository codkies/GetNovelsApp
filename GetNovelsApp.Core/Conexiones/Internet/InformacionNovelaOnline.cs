using System;
using System.Collections.Generic;

namespace GetNovelsApp.Core.Conexiones.Internet
{
    /// <summary>
    /// Modelo que utiliza la DB para obtener info acerca de una novela online.
    /// </summary>
    public class InformacionNovelaOnline
    {
        string getNacionalidad = $"select NacionID from Naciones where NacionNombre = x";
        string insertNacion = $"insert into Naciones (nombreNacion)";

        string getAutor = "select AutorID from Autores where NombreAutor = x";
        string insertAutor = $"insert into Autores (nombre, nacionalidadID)";

        string getNov = $"";
        string insertNov = $"insert into Novelas " +
                    $"(AutorID, NovelaTitulo) values (x, y)";


        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo;

        public string Autor;

        public string Nacionalidad;

        public bool HistoriaCompletada;

        public bool TraduccionCompletada;

        public float Review;

        public float CantidadReviews;

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

        public InformacionNovelaOnline(string q, string titulo, string autor, bool historiaCompletada, bool traduccionCompletada, float review, float cantidadReviews, List<string> generos, Uri linkPrincipal, List<Uri> linksDeCapitulos, Uri imagen, string sipnosis, List<string> tags)
        {
            this.q = q;
            Titulo = titulo;
            Autor = autor;
            HistoriaCompletada = historiaCompletada;
            TraduccionCompletada = traduccionCompletada;
            Review = review;
            CantidadReviews = cantidadReviews;
            Generos = generos;
            LinkPrincipal = linkPrincipal;
            LinksDeCapitulos = linksDeCapitulos;
            Imagen = imagen;
            Sipnosis = sipnosis;
            Tags = tags;
        }
    }
}
