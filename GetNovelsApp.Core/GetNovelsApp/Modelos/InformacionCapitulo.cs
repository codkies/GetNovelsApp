namespace GetNovelsApp.Core.Modelos
{
    public struct InformacionCapitulo
    {
        public InformacionCapitulo(string tituloCapitulo, int valor, float numeroCapitulo)
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
