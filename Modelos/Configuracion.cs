using System.Collections.Generic;

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

    public Configuracion(List<string> xPaths, string pathCarpeta, int capitulosPorPdf)
    {
        this.xPaths = xPaths;
        PathCarpeta = pathCarpeta;
        CapitulosPorPdf = capitulosPorPdf;
    }
}


//iText7_Test(Test, "Novela X", 2, Path);