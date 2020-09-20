using System.Collections.Generic;
using GetNovelsApp.Core.Utilidades;
using iText.Layout.Element;

namespace GetNovelsApp.Core.Modelos
{
    public class Novela
    {
        /// <summary>
        /// Empieza a crear una novela.
        /// </summary>
        /// <param name="titulo"></param>
        /// <param name="ultimoCap"></param>
        public Novela(string titulo, string primerLink, int ultimoCap)
        {
            Titulo = titulo;
            CapitulosSinImprimir = new List<Capitulo>();
            CapitulosImpresos = new List<Capitulo>();
            UltimoCap = ultimoCap;
            PrimerLink = primerLink;
        }

        #region Fields

        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo;

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
        public int UltimoCap;

        public string PrimerLink;


        #endregion


       #region Props


        /// <summary>
        /// Primer capitulo de la novela. Encotrado segun el link Original.
        /// </summary>
        public int PrimerNumeroCapitulo => ManipuladorDeLinks.EncuentraInformacionCapitulo(PrimerLink).NumeroCapitulo;


        public bool HayCapitulosPorImprimir => CapitulosSinImprimir.Count > 0;


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