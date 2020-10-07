using System; 
using System.Security.AccessControl;
using System.Security.Permissions;
using GetNovelsApp.Core.Conexiones;
using GetNovelsApp.Core.Conexiones.Internet;

namespace GetNovelsApp.Core.Modelos
{
    public class Capitulo
    {
        #region Constructor & setups       

        public Capitulo(string Link, string Texto, string TituloCapitulo, Int64 NumeroCapitulo, Int64 Valor)
        {
            this.Link = new Uri(Link);

            if(Texto != null)
            {
                if (!Texto.Equals(""))
                {
                    this.Texto = Texto;
                }
            }

            if (!TituloCapitulo.Equals(""))
            {
                this.TituloCapitulo = TituloCapitulo;
            }

            if (NumeroCapitulo >= 0)
            {
                this.NumeroCapitulo = (float)NumeroCapitulo;
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
            Link = info.Link;
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

        /// <summary>
        /// Usado por WPF Binding
        /// </summary>
        public string LinkString => Link.ToString();


        public bool Descargado => Texto != null;



        #endregion
    }
}