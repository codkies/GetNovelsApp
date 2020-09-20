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
            CapitulosEnNovela = new List<Capitulo>();
            UltimoCap = ultimoCap;
            PrimerLink = primerLink;
        }

        #region Fields

        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo;

        /// <summary>
        /// Capitulos presentes en esta novela.
        /// </summary>
        public List<Capitulo> CapitulosEnNovela { get; private set; }
        
        /// <summary>
        /// Capitulo final especificado por el usuario.
        /// </summary>
        public int UltimoCap;

        public string PrimerLink { get; private set; }


        #endregion

        #region Props

        /// <summary>
        /// Primer capitulo de la novela. Encotrado segun el link Original.
        /// </summary>
        public int PrimerNumeroCapitulo => ManipuladorDeLinks.EncuentraInformacionCapitulo(PrimerLink).NumeroCapitulo;
        

        #endregion

        public void AgregaCapitulo(Capitulo capituloNuevo)
        {
            CapitulosEnNovela.Add(capituloNuevo);
        }
    }
    
}