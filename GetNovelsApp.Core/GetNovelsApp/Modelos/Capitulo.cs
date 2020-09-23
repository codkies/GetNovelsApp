using System.Security.AccessControl;
using GetNovelsApp.Core.Utilidades;

namespace GetNovelsApp.Core.Modelos
{
    public class Capitulo  
    {
        #region Constructor
        
        /// <summary>
        /// Crea un capitulo con su texto
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="link"></param>
        public Capitulo(string texto, string link)
        {
            Link = link;

            InformacionCapitulo info = ManipuladorDeLinks.EncuentraInformacionCapitulo(link);
            TituloCapitulo = info.TituloCapitulo;
            NumeroCapitulo = info.NumeroCapitulo;
            Valor = info.Valor;

            Texto = texto;
        }


        #endregion

        #region Fields

        /// <summary>
        /// Lo que compone este capitulo.
        /// </summary>
        public readonly string Texto;

        /// <summary>
        /// El numero de este capitulo.
        /// </summary>
        public readonly float NumeroCapitulo;

        /// <summary>
        /// El titulo de este capitulo.
        /// </summary>
        public readonly string TituloCapitulo;

        /// <summary>
        /// Link a este capitulo.
        /// </summary>
        public readonly string Link;

        /// <summary>
        /// Cuantos valores numericos toma este capitulo. 99% veces este valor = 1
        /// </summary>
        public readonly int Valor;

        #endregion

        #region Propieades

        /// <summary>
        /// Los capitulos especiales, toman mas de 1 valor numerico.
        /// </summary>
        public bool Especial => Valor > 1;


        /// <summary>
        /// Cantidad de caracteres en este capitulo.
        /// </summary>
        public int Caracteres => Texto.Length;


        /// <summary>
        /// Revisa que todas las propieades de este capitulo esten establecidas..
        /// </summary>
        public bool CapituloLleno
        {
            get
            {
                bool textoLleno = !Texto.Equals(string.Empty);
                bool tituloCapLleno = !TituloCapitulo.Equals(string.Empty);
                bool linkEstablecido = !Link.Equals(string.Empty);

                bool valorEstablecido = Valor > 0;
                bool numeroEstablecido = NumeroCapitulo > 0;
                return textoLleno & tituloCapLleno & linkEstablecido & valorEstablecido & numeroEstablecido;
            }
        } 

        #endregion
    }
}