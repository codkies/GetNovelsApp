namespace GetNovelsApp.Core.Modelos
{
    public struct Capitulo
    {
        public string Texto;
        public int NumeroCapitulo;

        public int Caracteres => Texto.Length;

        public Capitulo(string texto, int numeroCapitulo)
        {
            Texto = texto;
            NumeroCapitulo = numeroCapitulo;
        }
    }
}