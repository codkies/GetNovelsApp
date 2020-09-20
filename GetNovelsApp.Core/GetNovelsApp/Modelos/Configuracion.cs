using System.Collections.Generic;

namespace GetNovelsApp.Core.Modelos
{
    public struct Configuracion
    {
        /// <summary>
        /// Capitulos por pdf
        /// </summary>
        public int CapitulosPorPdf;

        /// <summary>
        /// Lista de xPaths a revisar.
        /// </summary>
        public List<string> xPaths;

        /// <summary>
        /// Carpeta donde guardar la novela.
        /// </summary>
        public string PathCarpeta;


        /// <summary>
        /// Lista de xPaths para encontrar el boton de pasar página.
        /// </summary>
        public List<string> xPathsBotonSiguiente;

        public Configuracion(List<string> xPaths, List<string> xPathsBotonSiguiente, string pathCarpeta, int capitulosPorPdf)
        {
            this.xPathsBotonSiguiente = xPathsBotonSiguiente;
            this.xPaths = xPaths;
            PathCarpeta = pathCarpeta;
            CapitulosPorPdf = capitulosPorPdf;
        }
    }
}


//iText7_Test(Test, "Novela X", 2, Path);