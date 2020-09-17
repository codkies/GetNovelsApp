using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        MyScraper scraper = new MyScraper();
        ContructorDePdf constructor = new ContructorDePdf();

        //Informacion de novela
        Console.WriteLine("Titulo:");
        string Titulo = Console.ReadLine();
        //string Titulo = "Versatile Mage";

        Console.WriteLine("Link:");
        string link = Console.ReadLine();
        //string link = "https://www.readlightnovel.org/versatile-mage/chapter-1";

        Console.WriteLine("Primer capitulo:");
        int empiezaEn = int.Parse((Console.ReadLine()));
        //int empiezaEn = 1;

        Console.WriteLine("Ultimo capitulo:");
        int terminaEn = int.Parse((Console.ReadLine()));
        //int terminaEn = 1500;

        Console.WriteLine("Capitulos por PDF:");
        int capitulosPorPdf = int.Parse((Console.ReadLine()));
        //int capitulosPorPdf = 5;

        Console.WriteLine("Carpeta:");
        string Path = Console.ReadLine();
        //string Path = "C:\\Users\\Juan\\Desktop\\Novelas\\";


        Console.WriteLine("\nPresiona enter para confirmar.");
        Console.ReadLine();
        //---------------------------

        //Configuracion de Scraper
        string xPath = "//*[@class = 'desc']/p";
        //---------------------------

        //Preparaciones
        constructor.InicializaConstructor(Titulo, capitulosPorPdf, Path);
        scraper.InicializaScrapper(xPath);
        //---------------------------

        for (int i = empiezaEn; i < terminaEn + 1; i++)
        {
            string Capitulo = scraper.ObtenCapitulo(link, i);
            Console.WriteLine($"Program --> {i} capitulo obtenido. ///");
            constructor.AgregaCapitulo(Capitulo);

            if (scraper.HayOtroCapitulo)
            {
                link = scraper.SiguienteCapitulo;
            }
            else
            {
                constructor.FinalizoNovela();
                Console.WriteLine($"Program--> Ultimo capitulo: {i}. !!!");
                break;
            }
        }

        MustraResultado(scraper, constructor);
    }

    private static void MustraResultado(MyScraper scraper, ContructorDePdf constructor)
    {
        Console.WriteLine($"Constructor--> Finalizado:\n" +
                    $"Se han creado {constructor.DocumentosCreados}.\n" +
                    $"Se han ignorado {scraper.EntradasIgnoradas} entradas.\n" +
                    $"Se han obtenido {scraper.CapitulosEncontrados}.\n" +
                    $"Total de {scraper.CaracteresVistos}." +
                    $"Los PDFs están en: {constructor.Path}");

        Console.WriteLine("press Enter to exit.");
        Console.ReadLine();
    }
}

//List<string> Test = new List<string>()
//{
//    "Soy cap 1. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
//    "Soy cap 2. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
//    "Soy cap 3. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
//    "Soy cap 4. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
//    "Soy cap 5. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios",
//    "Soy cap 6. Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola AdiosHey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios Hey Hola Adios"
//};
//iText7_Test(Test, "Novela X", 2, Path);