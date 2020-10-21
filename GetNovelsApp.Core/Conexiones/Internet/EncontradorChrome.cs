using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace GetNovelsApp.Core.Conexiones.Internet
{
    internal class EncontradorChrome
    {
        readonly string xPathToLinks;
        readonly string xPathsToNxtPgBtn;
        readonly string xPathsToTitle;


        List<ChromeDriver> chromeDrivers = new List<ChromeDriver>();
      

        public EncontradorChrome(string xPathToLinks, string xPathsToNxtPgBtn, string xPathsToTitle)
        {
            this.xPathToLinks = xPathToLinks;
            this.xPathsToNxtPgBtn = xPathsToNxtPgBtn;
            this.xPathsToTitle = xPathsToTitle;
        }


        /// <summary>
        /// Version simple del metodo
        /// </summary>
        /// <param name="linkNovela"></param>
        /// <param name="pathBotonesOpcionales"></param>
        /// <returns></returns>
        public List<string> EncuentraInfoNovela(string linkNovela, string pathBotonesOpcionales = null)
        {
            return EncuentraInfoNovela(linkNovela, new List<string>() { pathBotonesOpcionales });
        }

        /// <summary>
        /// Version compleja
        /// </summary>
        /// <param name="linkNovela"></param>
        /// <param name="pathBotonesOpcionales"></param>
        /// <returns></returns>
        public List<string> EncuentraInfoNovela(string linkNovela, List<string> pathBotonesOpcionales = null)
        {

            using var browser = ObtenChrome();

            //Ve al link
            try
            {
                browser.Navigate().GoToUrl(linkNovela);
            }
            catch (Exception)
            {
                return null; 
            }

            //presiona el boton opcional
            if(pathBotonesOpcionales != null)
            {
                foreach (string path in pathBotonesOpcionales)
                {
                    var chaptersBtn = browser.FindElementByXPath(path);
                    TryClick(chaptersBtn);
                }
            }

            var titulo = EncuentraTitulo(browser);
            List<string> Capitulos = EncuentraCaps(browser);

            FlushDrivers();
            return Capitulos;
        }


        #region Encontradores


        private string EncuentraTitulo(ChromeDriver browser)
        {
            var htmlTitulo = browser.FindElementsByXPath(xPathsToTitle).First();
            return TryInnerText(htmlTitulo);
        } 


        private List<string> EncuentraCaps(ChromeDriver browser)
        {
            List<string> Output = new List<string>();
            bool hayPaginaPorTomar;
            do
            {
                //caps presentes                
                int antes = Output.Count;
                while (antes == Output.Count)
                {
                    ReadOnlyCollection<IWebElement> elementosLinks = browser.FindElementsByXPath(xPathToLinks);
                    var nuevosLinks = new List<string>();
                    bool agregalos = true;
                    foreach (var l in elementosLinks)
                    {
                        string link = TryAttribute(l, "href");
                        if (string.IsNullOrEmpty(link) | Output.Contains(link))
                        {
                            agregalos = false;
                            break;
                        }
                        nuevosLinks.Add(link);
                    }

                    if (agregalos) Output.AddRange(nuevosLinks);
                }

                //pasando a la siguiente pagina
                IWebElement siguientePaginaBtn;
                try
                {
                    siguientePaginaBtn = browser.FindElementByXPath(xPathsToNxtPgBtn);
                    hayPaginaPorTomar = true;
                    TryClick(siguientePaginaBtn);
                }
                catch (Exception ex)
                {
                    hayPaginaPorTomar = false;
                    Console.WriteLine("\nSe acabaron los capitulos.");
                }

                Console.WriteLine($"\nObtenidos {Output.Count} links");
                Console.WriteLine($"Ultimo Link: {Output.Last()}");

            }
            while (hayPaginaPorTomar);

            return Output;
        }


        #endregion


        #region Shorthands 
        /*metodos para acortar el tamaño del metodo principal*/

        /// <summary>
        /// Obtiene un WebDriver
        /// </summary>
        /// <returns></returns>
        private ChromeDriver ObtenChrome()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArgument("headless");

            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;

            var browser = new ChromeDriver(chromeDriverService, options);
            chromeDrivers.Add(browser);

            return browser;
            //return new ChromeDriver();
        }


        public void FlushDrivers()
        {
            for (int i = chromeDrivers.Count; i-- < 0;)
            {
                var driver = chromeDrivers[i];
                driver.Quit();
            }

            chromeDrivers.Clear();
        }



        /// <summary>
        /// Intenta presionar el boton el numero de veces especificado
        /// </summary>
        /// <param name="button"></param>
        /// <param name="maxAttemps"></param>
        /// <returns></returns>
        private static bool TryClick(IWebElement button, int maxAttemps = 10)
        {
            int attemps = 0;
            bool result = false;
            do
            {
                try
                {
                    button.Click();
                    result = true;
                    break;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"No se encontró el boton. Reintentando...\n" +
                    //                    $"Error: {ex.Message}");
                    attemps++;
                }

            } while (attemps < maxAttemps);

            return result;
        }


        /// <summary>
        /// Intenta encontrar el atributo el numero de veces especificado
        /// </summary>
        /// <param name="elemento"></param>
        /// <param name="attribute"></param>
        /// <param name="maxAttemps"></param>
        /// <returns></returns>
        private static string TryAttribute(IWebElement elemento, string attribute, int maxAttemps = 10)
        {
            int attemps = 0;
            string result = null;
            do
            {
                try
                {
                    result = elemento.GetAttribute(attribute);
                    break;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"No se encontró el link. Reintentando...\n" +
                    //                    $"Error: {ex.Message}");
                    attemps++;
                }

            } while (attemps < maxAttemps);

            return result;
        }


        private static string TryInnerText(IWebElement elemento, int maxAttemps = 10)
        {
            int attemps = 0;
            string result = null;
            do
            {
                try
                {
                    result = elemento.Text;
                    break;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"No se encontró el link. Reintentando...\n" +
                    //                    $"Error: {ex.Message}");
                    attemps++;
                }

            } while (attemps < maxAttemps);

            return result;
        }


        #endregion
    }
}
