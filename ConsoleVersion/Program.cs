using System;
using System.Collections.Generic;
using GetNovelsApp.Modelos;

namespace GetNovelsApp.ConsoleVersion
{
    class Program
    {
        static void Main(string[] args)
        {
            Scraper scraper = new Scraper();
            PdfConstructor constructor = new PdfConstructor();
            List<string> Test = new List<string>()
        {
            "Soy cap 1. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola.Punto.\n\n Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios.Punto.\n\n\n Hey Hola Adios Hey Hola Adios Hey.Punto.\n\b Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios. Final de entrada.",
            "Soy cap 2. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios. Final de entrada.",
            "Soy cap 3. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
            "Soy cap 4. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
            "Soy cap 5. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
            "Soy cap 6. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios"
        };

            //Informacion de novela       
            //MuestraInput("Titulo:", out string Titulo);        
            //MuestraInput("Link del primer capitulo:", out string link);
            //MuestraInput("Primer capitulo:", out string primerCap);
            //MuestraInput("Ultimo capitulo:", out string ultimoCap);
            //MuestraInput("Capitulos por PDF:", out string capsPorPDF);
            //MuestraInput("Carpeta:", out string Path);

            //int capitulosPorPdf = int.Parse(capsPorPDF);
            //int empiezaEn = int.Parse(primerCap);
            //int terminaEn = int.Parse(ultimoCap);


            /*Strongest Abandoned son*/
            //string Titulo = "Strongest Abandoned Son";
            //string link = "https://wuxiaworld.site/novel/strongest-abandoned-son/chapter-301";
            //int empiezaEn = 301;
            //int terminaEn = 2257;

            /*Only I level up*/
            string Titulo = "Only I level up";
            string link = "https://wuxiaworld.site/novel/only-i-level-up-novel-solo-leveling-completed/chapter-185";
            int empiezaEn = 186;
            int terminaEn = 270;

            int capitulosPorPdf = 10;
            string Path = "C:\\Users\\Juan\\Desktop\\Novelas\\";



            Mensajero.MuestraNotificacion($"Titulo: {Titulo}\n" +
                                            $"Link: {link}\n" +
                                            $"PrimerCap: {empiezaEn}\n" +
                                            $"UltimoCap: {terminaEn}\n" +
                                            $"CapitulosPorPdf: {capitulosPorPdf}\n" +
                                            $"Carpeta: {Path}");

            MuestraInput("\n... Presiona enter para confirmar.", out string _, ColorTitulo: ConsoleColor.Red);
            //---------------------------

            //Configuracion de Scraper
            List<string> xPaths = new List<string>()
        {
            "//*[@class = 'desc']/p",
            "//div[@class = 'cha-words']/p",
            "//div[@class = 'text-left']/p"
        };

            Novela novela = new Novela(Titulo, link, terminaEn, empiezaEn);
            Configuracion configuracion = new Configuracion(xPaths, Path, capitulosPorPdf);
            //---------------------------

            //Preparaciones
            constructor.InicializaConstructor(novela, configuracion);
            scraper.InicializaScrapper(configuracion);
            //---------------------------       

            //for (int i = 0; i < Test.Count; i++)
            //{
            //    constructor.AgregaCapitulo(Test[i]);
            //}
            //constructor.FinalizoNovela();

            for (int indexCap = empiezaEn; indexCap < terminaEn + 1; indexCap++)
            {
                Capitulo Capitulo = scraper.ObtenCapitulo(link, indexCap);
                constructor.AgregaCapitulo(Capitulo);

                //Meter este como "Especial" en el Mensajero.
                Mensajero.MuestraEspecial($"Program --> {indexCap}/{terminaEn}");

                if (scraper.HayOtroCapitulo)
                {
                    link = scraper.SiguienteCapitulo;
                }
                else
                {
                    constructor.FinalizoNovela();
                    Mensajero.MuestraNotificacion($"Program--> Ultimo capitulo: {indexCap}.");
                    break;
                }
            }
            MustraResultado(scraper, constructor);
        }

        private static void MuestraInput(string titulo, out string Obten, ConsoleColor ColorTitulo = ConsoleColor.Cyan, ConsoleColor ColorEscrito = ConsoleColor.White)
        {
            Console.ForegroundColor = ColorTitulo;
            Console.WriteLine(titulo);
            Console.ForegroundColor = ColorEscrito;
            Obten = Console.ReadLine();
            Console.WriteLine("\b");
        }

        private static void MustraResultado(Scraper scraper, PdfConstructor constructor)
        {
            Mensajero.MuestraExito($"Constructor--> Finalizado:\n" +
                        $"Se han creado {constructor.DocumentosCreados}.\n" +
                        $"Se han ignorado {scraper.EntradasIgnoradas} entradas.\n" +
                        $"Se han obtenido {scraper.CapitulosEncontrados}.\n" +
                        $"Total de {scraper.CaracteresVistos}." +
                        $"Los PDFs están en: {constructor.Path}");

            Console.WriteLine("press Enter to exit.");
            Console.ReadLine();
        }
    }
}


//iText7_Test(Test, "Novela X", 2, Path);