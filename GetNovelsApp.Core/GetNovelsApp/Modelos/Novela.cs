using System.Collections.Generic;
using System.Runtime.InteropServices;
using GetNovelsApp.Core.Utilidades;

namespace GetNovelsApp.Core.Modelos
{
    public class Novela
    {
        public Novela(string link, string FolderPath)
        {
            CarpetaPath = FolderPath;
            InformacionNovela info = ManipuladorDeLinks.EncuentraInformacionNovela(link);

            Titulo = info.Titulo;
            CarpetaPath += $"\\{Titulo}\\";
            LinkPaginaPrincipal = info.LinkPaginaPrincipal;

            LinksDeCapitulos = info.LinksDeCapitulos;
            PrimerLink = info.PrimerLink;
            UltimoLink = info.UltimoLink;

            PrimerNumeroCapitulo = info.PrimerCapitulo;
            UltimoNumeroCapitulo = info.UltimoCapitulo;

            CapitulosSinImprimir = new List<Capitulo>();
            CapitulosImpresos = new List<Capitulo>();
        }       


        #region Fields

        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo;


        /// <summary>
        /// Direccion a la carpeta donde se guardará la novela
        /// </summary>
        public string CarpetaPath;

        /// <summary>
        /// Link a su pagina principal de la novela
        /// </summary>
        public string LinkPaginaPrincipal;


        /// <summary>
        /// Lista de todos los links de los capitulos
        /// </summary>
        public List<string> LinksDeCapitulos;


        /// <summary>
        /// Link del primer capitulo.
        /// </summary>
        public string PrimerLink;


        /// <summary>
        /// Link del ultimo capitulo.
        /// </summary>
        public string UltimoLink;


        #endregion


        #region Props

        /// <summary>
        /// Capitulos presentes en esta novela que no han sido metidos en el PDF
        /// </summary>
        public List<Capitulo> CapitulosSinImprimir { get; private set; }


        /// <summary>
        /// Capitulos presentes en esta novela que ya han sido metidos en el PDF
        /// </summary>
        public List<Capitulo> CapitulosImpresos { get; private set; }


        /// <summary>
        /// Capitulo final especificado por el usuario.
        /// </summary>
        public int UltimoNumeroCapitulo { get; private set; }


        /// <summary>
        /// Primer capitulo de la novela. Encotrado segun el link Original.
        /// </summary>
        public int PrimerNumeroCapitulo { get; private set; }


        public bool TengoCapitulosPorImprimir => CapitulosSinImprimir.Count > 0;


        #endregion


        /// <summary>
        /// Agrega un capitulo a la novela.
        /// </summary>
        /// <param name="capituloNuevo"></param>
        public void AgregaCapitulo(Capitulo capituloNuevo)
        {
            CapitulosSinImprimir.Add(capituloNuevo);
        }


        /// <summary>
        /// Informa que X capitulo ha sido impreso.
        /// </summary>
        /// <param name="capitulo"></param>
        public void InformaCapitulosImpresos(Capitulo capitulo)
        {
            CapitulosSinImprimir.Remove(capitulo);
            CapitulosImpresos.Add(capitulo);
        }
    }
}