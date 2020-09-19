namespace GetNovelsApp.Core.Modelos
{
    public struct Novela
    {
        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo;

        /// <summary>
        /// Link a la novela
        /// </summary>
        public string link;

        /// <summary>
        /// Primer capitulo de la novela. Tipicamente es 1
        /// </summary>
        public int PrimerCap;

        /// <summary>
        /// Ultimo capitulo de la novela
        /// </summary>
        public int UltimoCap;

        public Novela(string titulo, string link, int ultimoCap, int primerCap = 1)
        {
            Titulo = titulo;
            this.link = link;
            UltimoCap = ultimoCap;
            PrimerCap = primerCap;
        }
    }
}


//iText7_Test(Test, "Novela X", 2, Path);