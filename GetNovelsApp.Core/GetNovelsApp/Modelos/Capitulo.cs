using System;
using System.Security.AccessControl;
using GetNovelsApp.Core.Conexiones;
using GetNovelsApp.Core.Conexiones.Internet;

namespace GetNovelsApp.Core.Modelos
{
    public class Capitulo  
    {
        #region Constructor & setups       
      
        /// <summary>
        /// DO NOT DELETE! For the DB!!!
        /// </summary>
        /// <param name="Link"></param>
        /// <param name="TextoCapitulo"></param>
        /// <param name="Titulo"></param>
        /// <param name="Numero"></param>
        /// <param name="Valor"></param>
        public Capitulo(string Link, string TextoCapitulo = "", string Titulo = "", Int64 Numero = -1, Int64 Valor = -1)
        {
            this.Link = new Uri(Link);

            if (!TextoCapitulo.Equals(""))
            {
                Texto = TextoCapitulo;
            }

            if (!Titulo.Equals(""))
            {
                TituloCapitulo = Titulo;
            }

            if(Numero >= 0)
            {
                NumeroCapitulo = (float)Numero;
            }

            if (Valor >= 0)
            {
                this.Valor = (int)Valor;
            }
        }

        public Capitulo(Uri link)
        {
            Link = link;
        }

        public Capitulo(CapituloWebModel info)
        {
            TituloCapitulo = info.TituloCapitulo;
            NumeroCapitulo = info.NumeroCapitulo;
            Valor = info.Valor;
        }

        public void UpdateTexto(string texto)
        {
            Texto = texto;
        }

        #endregion


        #region Fields

        /// <summary>
        /// Lo que compone este capitulo.
        /// </summary>
        public string Texto { get; private set; }

        /// <summary>
        /// El numero de este capitulo.
        /// </summary>
        public float NumeroCapitulo { get; private set; }

        /// <summary>
        /// El titulo de este capitulo.
        /// </summary>
        public string TituloCapitulo { get; private set; }

        /// <summary>
        /// Link a este capitulo.
        /// </summary>
        public readonly Uri Link;

        /// <summary>
        /// Cuantos valores numericos toma este capitulo. 99% veces este valor = 1
        /// </summary>
        public int Valor { get; private set; }

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



        #endregion
    }
}