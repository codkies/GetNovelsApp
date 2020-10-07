using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.WPF.Models
{
    /// <summary>
    /// Establece los requisitos para que la app corra.
    /// </summary>
    public static class Setter
    {
        /// <summary>
        /// Referencia al GetNovels que se está usando.
        /// </summary>
        private static GetNovels app;
        public static IPath wb;

        public static GetNovels ObtenGetNovel()
        {
            wb = GetWebsite();

            IComunicador com = new ComunicadorWPF();

            IFabrica fb = new FactoryWPF(); 

            app = new GetNovels(fb, com);
            return app;
        }


        private static Website GetWebsite()
        {
            string dominio = "wuxiaworld.site";

            List<string> textos = new List<string>()
            {
                "//div[@class = 'cha-words']/p",
                "//div[@class = 'text-left']/p",
                "//div[@class = 'text-content']/p",
                "//div[@class='text-left']/div/p",
                "//div[@class='cha-words']/div/div/p",
                "//div[@class='entry-content content']/p",
                "//div[@id='divReadContent']/p"
            };


            List<string> links = new List<string>()
            {
                "//ul[@class= 'main version-chap active']/li/a",
                "//li[@class= 'wp-manga-chapter  ']/a"
            };


            List<string> titulo = new List<string>()
            {
                "//div[@class='post-title']/*",
                "//h3"
            };
            

            return new Website(dominio, links, textos, titulo, Core.Conexiones.DB.OrdenLinks.Descendiente);
        }

    }
}
