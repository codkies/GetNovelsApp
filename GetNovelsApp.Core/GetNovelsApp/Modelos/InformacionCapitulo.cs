namespace GetNovelsApp.Core.Utilidades
{
    public struct InformacionCapitulo
    {
        public string TituloCapitulo;
        public int Valor;
        public int NumeroCapitulo;

        public InformacionCapitulo(string tituloCapitulo, int valor, int numeroCapitulo)
        {
            TituloCapitulo = tituloCapitulo;
            Valor = valor;
            NumeroCapitulo = numeroCapitulo;
        }
    }
}
