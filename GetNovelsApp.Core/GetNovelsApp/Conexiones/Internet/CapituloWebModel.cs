namespace GetNovelsApp.Core.Conexiones.Internet
{
    /// <summary>
    /// Informacion basica de un capitulo. Esta info se obtiene online. No incluye el texto.
    /// </summary>
    public struct CapituloWebModel
    {
        public CapituloWebModel(string tituloCapitulo, int valor, float numeroCapitulo)
        {
            TituloCapitulo = tituloCapitulo;
            Valor = valor;
            NumeroCapitulo = numeroCapitulo;
        }

        public string TituloCapitulo;
        public int Valor;
        public float NumeroCapitulo;
    }
}
