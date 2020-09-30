using System;
using System.Collections.Generic;
using System.Globalization;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Reportaje;

namespace GetAppsNovel.ConsoleVersion
{
    /*
    1) hacer que regrese diferentes xPaths dependiendo de la URL de la novela
    2) hacer que contenga un IMensajero y que se lo pase al mensajero global
    3) cambiar el nombre del mensajero global
    4) borrar propiedades que no están en uso
     */
    public class ConfiguracionConsoleUI : ConfiguracionBasica
    {
        public ConfiguracionConsoleUI(ConsoleUI UI, IFabrica fabrica, string DireccionDiscoDuro, int BatchSize, int CapitulosPorDoc)
        {
            InicializaConfiguracion();

            _fabrica = fabrica;
            _tamañoBatch = BatchSize;
            _direccionDiscoDuro = DireccionDiscoDuro;
            _capsPorDocumento = CapitulosPorDoc;
            ConsoleUI = UI;
        }


        private void InicializaConfiguracion()
        {
            /*El orden es importante*/
            List<string> textos = new List<string>()
            {
                "//div[@class = 'cha-words']/p",
                "//div[@class = 'text-left']/p",
                "//div[@class = 'text-content']/p",
                "//div[@class='text-left']/div/p",
                "//div[@class='cha-words']/div/div/p",
                "//div[@class='entry-content content']/p",
                "//div[@id='divReadContent']/p",
                "//*[@class = 'desc']/p"
            };

            List<string> nextBtn = new List<string>()
            {
                "//div[@class='nav-next']/a", //Wuxiaworldsite
                "//li/a[@class='next next-link']", //readlightnovels
            };

            List<string> links = new List<string>()
            {
                "//ul[@class= 'main version-chap active']/li/a",
                "//li[@class= 'wp-manga-chapter  ']/a"
            };

            List<string> titulos = new List<string>()
            {
                "//div[@class='post-title']/*",
                "//h3"
            };

            List<string> sipnosis = new List<string>()
            {
                "//div[@id='editdescription']/p"
            };

            List<string> imagen = new List<string>()
            {
                "//div[@class='seriesimg']/img"
            };

            List<string> tags = new List<string>()
            {
                "//div[@id='showtags']/a"
            };

            wuxiaworld_site = new Website(dominio, links, nextBtn, textos, titulos, sipnosis, imagen, tags);
        }


        #region Fields

        /// <summary>
        /// Referencia publica a la consola del programa.
        /// </summary>
        public readonly ConsoleUI ConsoleUI;

        Uri dominio = new Uri("https://wuxiaworld.site/");      

        public int _tamañoBatch;
        public int _capsPorDocumento;
        public string _direccionDiscoDuro;

        public Website wuxiaworld_site;

        IFabrica _fabrica;


        #endregion


        #region Contrato privado (ConfiguracionBasica inheritence)

        protected override int capsPorDocumento => _capsPorDocumento; 

        protected override int tamañoBatch => _tamañoBatch;


        protected override IPath xPaths => wuxiaworld_site;

        protected override IComunicador comunicador => ConsoleUI;

        protected override string direccionDiscoDuro => _direccionDiscoDuro;

        protected override IFabrica fabrica => _fabrica;


        #endregion
    }
}
