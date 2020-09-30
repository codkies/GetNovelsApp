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

        public static GetNovels ObtenGetNovel()
        {
            IPath wb = GetWebsite();

            IComunicador com = new ComunicadorWPF();

            IFabrica fb = new FactoryWPF(); 
            
            string Folder = "C:\\Users\\Juan\\Desktop\\Novelas";
            IConfig IConfig = new ConfiguracionWPF(wb, fb, com, Folder, 25, 0);

            return new GetNovels(IConfig);
        }

        private static Website GetWebsite()
        {
            Uri dominio = new Uri("https://wuxiaworld.site");
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

            List<string> titulo = new List<string>()
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

            return new Website(dominio, links, nextBtn, textos, titulo, sipnosis, imagen, tags);
        }

    }
}
