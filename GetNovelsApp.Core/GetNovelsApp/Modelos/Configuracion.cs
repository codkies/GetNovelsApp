using System.Collections.Generic;

namespace GetNovelsApp.Core.Modelos
{
    public static class Configuracion
    {
        /// <summary>
        /// Capitulos por pdf.
        /// </summary>
        public static int CapitulosPorPdf { get; private set; } = 100;


        /// <summary>
        /// Lista de xPaths a revisar.
        /// </summary>
        public static List<string> xPathsTextos { get; private set; } = new List<string>()
            {
                "//div[@class = 'cha-words']/p",
                "//div[@class = 'text-left']/p",
                "//*[@class = 'desc']/p"
            };


        /// <summary>
        /// Lista de xPaths para conseguir los botones next.
        /// </summary>
        public static List<string> xPathsSiguienteBoton { get; private set; } = new List<string>()
            {
                "//div[@class='nav-next']/a", //Wuxiaworldsite
                "//li/a[@class='next next-link']", //readlightnovels

            };


        /// <summary>
        /// Lista de xPaths para conseguir los titulos.
        /// </summary>
        public static List<string> xPathsTitulo { get; private set; } = new List<string>()
            {
                "//h3", 
                "//h1"
            };


        /// <summary>
        /// Lista de xPaths para conseguir los links de los capitulos.
        /// </summary>
        public static List<string> xPathsLinks { get; private set; } = new List<string>()
            {
                "//ul[@class= 'main version-chap active']/li/a",
                "//li[@class= 'wp-manga-chapter  ']/a"
            };
    }

}