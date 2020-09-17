using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace ScrapperConsoleApp
{
    /*
    Ideas:
    - Meter un campo de maximos capitulos y una barra de carga con el % de cada uno
    - Agregar variabilidad en la fuente y en el tamaño
    - Casilla para elegir la carpeta
    - Uh, cómo colocar imagenes de portada?
    - Mayor soporte para más páginas
        - Hacer el xPath variable
        - Expandir la manera que se iteran las direcciones de los capitulos
    - Colocar variable las palabras claves de los checks
    */



    class Program
    {

        static void Main(string[] args)
        {
            //ScraperCore scrapperCore = new ScraperCore();

            //string Titulo = "Versatile Mage";
            //string link = "https://www.readlightnovel.org/versatile-mage/chapter-1";
            //string xPath = "//*[@class = 'desc']/p";
            //int empiezaEn = 1;
            //int terminaEn = 2;

            //List<string> Caps = scrapperCore.ObtenNovela(link, xPath, empiezaEn, terminaEn);
            //ConstruyePDF(Caps, 50, Titulo);

            //Path a donde se crearan los PDFs
            //string Path = "";
            List<string> Test = new List<string>()
            {
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
                "Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios"
            };
            ConstruyePDFs(Test, 10, "Testing");
        }

        private static void ConstruyePDFs(List<string> Caps, int capitulosPorPDF, string titulo)
        {
            int contador = 1;
            int capitulosEnPdf = 0;
            bool salvado;

            PdfDocument pdf = new PdfDocument();
            PdfPage paginaActual = pdf.AddPage();

            int tamañoFuente = 20;
            int PosY = 10;            
            int Margen = 5;
            int sumador = tamañoFuente + Margen;

            XGraphics graph = XGraphics.FromPdfPage(paginaActual);
            XFont font = new XFont("Verdana", tamañoFuente, XFontStyle.Regular);
            XUnit disponible = paginaActual.Height;

            foreach (string capitulo in Caps)
            {
                if (capitulosEnPdf >= capitulosPorPDF)
                {
                    pdf.Save($"{titulo} - {contador}.pdf");                   

                    pdf = new PdfDocument();
                    paginaActual = pdf.AddPage();
                    graph = XGraphics.FromPdfPage(paginaActual);

                    contador++;
                    capitulosEnPdf = 0;
                    PosY = 10;
                } 

                if (disponible < sumador)
                {
                    paginaActual = pdf.AddPage();
                    graph = XGraphics.FromPdfPage(paginaActual);

                    disponible = paginaActual.Height;
                    PosY = 10;
                }

                graph.DrawString(capitulo, font, XBrushes.Black, new XRect(0, PosY, paginaActual.Width.Point, paginaActual.Height.Point), XStringFormats.TopLeft);

                PosY += sumador;
                disponible -= sumador;
                capitulosEnPdf++;
            }

            pdf.Save($"{titulo} - {contador}.pdf");
        }        
    }


    public class ScraperCore
    {
        /// <summary>
        /// Notifica cuando el proceso fue finalizado.
        /// </summary>
        public event Action<List<string>> CapitulosActualizados;


        /// <summary>
        /// Cantidad de entradas que no pasaron los checks.
        /// </summary>
        public int EntradasIgnoradas = 0;

        /// <summary>
        /// Caracteres mostrados
        /// </summary>
        public int caracteres;
        #region Core

        public List<string> ObtenNovela(string direccionPrimerCap, string xPath, int contador, int terminaEn)
        {
            List<string> capsDeNovela = new List<string>();

            string direccion = direccionPrimerCap;

            bool exito = true;
            caracteres = 0;

            Console.WriteLine($"--> Comenzando conexion");
            HtmlWeb conexion = new HtmlWeb();
            do
            {
                Console.WriteLine($"--> Scraping capitulo {contador}");
                string capitulo = ScrappDireccion(conexion, direccion, xPath, ref exito);
                if (!capitulo.Equals(""))
                {
                    capsDeNovela.Add(capitulo);
                    direccion = EncuentraSiguienteCap(direccion);

                    contador++;
                    caracteres += capitulo.Length;

                    Console.WriteLine($">-> Capitulo {contador} cargado. Tiene {capitulo.Length} caracteres.");
                    Console.WriteLine($">-> Yendo a: {direccion}");
                }
                else
                {
                    Console.WriteLine($"//> Capitulo {contador} tuvo un error. Deteniendo conexión.>>>>");
                }

            } while (exito & contador < terminaEn);

            Console.WriteLine($"--> Terminado conexión con {contador} capitulos y {caracteres} caracteres.");

            CapitulosActualizados?.Invoke(capsDeNovela);
            return capsDeNovela;
        }

        #endregion

        #region Helpers

        private string ScrappDireccion(HtmlWeb conexion, string direccion, string xPath, ref bool exito)
        {
            HtmlDocument doc = conexion.Load(direccion);

            List<string> CapituloDesordenado = new List<string>();
            foreach (var item in doc.DocumentNode.SelectNodes(xPath))
            {
                string entrada = item.InnerText;
                bool paso = RevisaEntrada(entrada);
                if (paso) CapituloDesordenado.Add(entrada);
            }

            exito = CapituloDesordenado.Count > 0;

            if (exito)
            {
                string capitulo = OrdenaCapitulo(CapituloDesordenado);
                return capitulo;
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// Revisa si una entrada pasa los Checks
        /// </summary>
        /// <param name="entrada"></param>
        /// <returns></returns>
        private bool RevisaEntrada(string entrada)
        {
            List<string> Checks = new List<string>()
            {
                "Edited", "Translated", "Editor", "Translator"
            };

            foreach (string checks in Checks)
            {
                if (entrada.Contains(checks))
                {
                    EntradasIgnoradas++;
                    return false;
                }
            }
            return true;
        }

        private string OrdenaCapitulo(List<string> capituloDesordenado)
        {
            string capituloOrdenado = string.Empty;
            foreach (string entrada in capituloDesordenado)
            {
                capituloOrdenado += $"{entrada}\n";
            }
            return capituloOrdenado;
        }


        private string EncuentraSiguienteCap(string direccionCapAnterior)
        {
            //Regresa "" si no encuentras nada.
            /*LINK: https://www.readlightnovel.org/versatile-mage/chapter-1   */

            string direccionNueva = string.Empty;
            string capitulo = string.Empty;

            for (int i = 0; i < direccionCapAnterior.Length; i++)
            {
                char letra = direccionCapAnterior[i];
                string letra_ = letra.ToString();
                bool EsUnNumero = char.IsDigit(letra);
                if (EsUnNumero)
                {
                    capitulo += letra.ToString(); //1

                    if (i == direccionNueva.Length - 1) break; //Si es el ultimo i, rompe el loop.

                    int end = i + 1;
                    for (int x = end; x < direccionCapAnterior.Length; x++)
                    {
                        char letraFutura = direccionCapAnterior[x];
                        if (char.IsDigit(letraFutura))
                        {
                            capitulo += letraFutura.ToString();//2
                            end = x;
                        }
                        else break;//Apenas halles una letra, rompe este loop.
                    }
                    capitulo = (int.Parse(capitulo) + 1).ToString(); //Conviertelo a INT, sumale 1 y metelo de nuevo en el link.
                    direccionNueva += capitulo;
                    i = end + 1 < direccionCapAnterior.Length - 1 ? end + 1 : direccionCapAnterior.Length;
                }
                else
                {
                    direccionNueva += letra.ToString();
                }

            }

            return direccionNueva;
        }

        #endregion
    }


}

