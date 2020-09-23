using System.Collections.Generic;
using System.Linq;

namespace GetNovelsApp.Core.Modelos
{
    public struct InformacionNovela
    {
        public InformacionNovela(string titulo, string linkPaginaPrincipal, List<string> linksDeCapitulos, float primerCapitulo, float ultimoCapitulo)
        {
            Titulo = titulo;
            LinkPaginaPrincipal = linkPaginaPrincipal;
            LinksDeCapitulos = linksDeCapitulos;
            PrimerCapitulo = primerCapitulo;
            UltimoCapitulo = ultimoCapitulo;
        }


        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo;

        /// <summary>
        /// Link a su pagina principal de la novela
        /// </summary>
        public string LinkPaginaPrincipal;


        /// <summary>
        /// Lista de todos los links de los capitulos
        /// </summary>
        public List<string> LinksDeCapitulos;


        /// <summary>
        /// Link de su primer capitulo
        /// </summary>
        public string PrimerLink => LinksDeCapitulos.First();

        /// <summary>
        /// Link de su ultimo capitulo
        /// </summary>
        public string UltimoLink => LinksDeCapitulos.Last();

        /// <summary>
        /// Numero de su primer capitulo
        /// </summary>
        public float PrimerCapitulo;

        /// <summary>
        /// Numero de su ultimo capitulo
        /// </summary>
        public float UltimoCapitulo;        
    }
}
