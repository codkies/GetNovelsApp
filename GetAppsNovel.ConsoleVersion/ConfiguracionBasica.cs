using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.ConfiguracionApp;

namespace GetAppsNovel.ConsoleVersion
{
    /*
    1) hacer que regrese diferentes xPaths dependiendo de la URL de la novela
    2) hacer que contenga un IMensajero y que se lo pase al mensajero global
    3) cambiar el nombre del mensajero global
    4) borrar propiedades que no están en uso
     */
    public class ConfiguracionBasica : IConfig
    {
        public int CapitulosPorPdf { get; } = 100;

        public List<string> xPathsTextos { get; } = new List<string>()
            {
                "//div[@class = 'cha-words']/p",
                "//div[@class = 'text-left']/p",
                "//div[@class='entry-content content']/p",
                "//div[@id='divReadContent']/p",
                "//*[@class = 'desc']/p"
            };

        public List<string> xPathsSiguienteBoton { get; } = new List<string>()
            {
                "//div[@class='nav-next']/a", //Wuxiaworldsite
                "//li/a[@class='next next-link']", //readlightnovels

            };

        public List<string> xPathsTitulo { get; } = new List<string>()
            {
                "//h3",
                "//h1"
            };

        public List<string> xPathsLinks { get; } = new List<string>()
            {
                "//ul[@class= 'main version-chap active']/li/a",
                "//li[@class= 'wp-manga-chapter  ']/a"
            };

        public int TamañoBatch { get; } = 25;
    }
}
