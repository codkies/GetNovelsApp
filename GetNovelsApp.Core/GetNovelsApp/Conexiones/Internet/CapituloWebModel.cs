using System;

namespace GetNovelsApp.Core.Conexiones.Internet
{
    /// <summary>
    /// Informacion basica de un capitulo. Esta info se obtiene online. No incluye el texto.
    /// </summary>
    public struct CapituloWebModel
    { 
        public CapituloWebModel(Uri Link, string tituloCapitulo, int valor, float numeroCapitulo)
        {
            this.Link = Link;
            TituloCapitulo = tituloCapitulo;
            Valor = valor;
            NumeroCapitulo = numeroCapitulo;
        }

        public Uri Link;
        public string TituloCapitulo;
        public int Valor;
        public float NumeroCapitulo;
    }
}
