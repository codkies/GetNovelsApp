namespace GetAppsNovel.ConsoleVersion
{
    class HayandoInfo
    {
        /*Probar estos:*/
        string xPathTitulo = "//div[@class ='post-title']/h3";
        string xPathPrimerCap = "//li[@class ='wp-manga-chapter  '][last()]/a"; //Lleva al Elemento. Hay que sacarle el Href.
        string xPathUltimoCap = "//li[@class ='wp-manga-chapter  ']";

    }
}


//iText7_Test(Test, "Novela X", 2, Path);