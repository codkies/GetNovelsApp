using System;

namespace GetNovelsApp.Core.Conexiones.Internet
{
    /// <summary>
    /// Informacion basica de un capitulo. Esta info se obtiene online. No incluye el texto.
    /// </summary>
    public class CapituloWebModel
    { 
        public CapituloWebModel(Uri Link, string tituloCapitulo, int valor, float numeroCapitulo)
        {
            this.Link = Link;
            TituloCapitulo = tituloCapitulo;
            Valor = valor;
            NumeroCapitulo = numeroCapitulo;
        }

        public CapituloWebModel(Uri link, string tituloCapitulo, float numeroCapitulo) 
        {
            Link = link;
            TituloCapitulo = tituloCapitulo;
            NumeroCapitulo = numeroCapitulo;
            Valor = 1;
        }

        public Uri Link;
        public string TituloCapitulo;
        public int Valor;
        public float NumeroCapitulo;
    }
}
