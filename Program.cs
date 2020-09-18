using System;
using System.Collections.Generic;
using System.Threading;

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

public static class Mensajero
{
    private const ConsoleColor ColorError = ConsoleColor.Red;
    private const ConsoleColor ColorNotificacion = ConsoleColor.DarkYellow;
    private const ConsoleColor ColorExito = ConsoleColor.DarkCyan;
    private const ConsoleColor ColorEspecial = ConsoleColor.White;

    public static void MuestraError(string mensaje)
    {
        Console.ForegroundColor = ColorError;
        Console.WriteLine(mensaje);
    }

    public static void MuestraNotificacion(string mensaje)
    {
        Console.ForegroundColor = ColorNotificacion;
        Console.WriteLine(mensaje);
    }

    public static void MuestraExito(string mensaje)
    {
        Console.ForegroundColor = ColorExito;
        Console.WriteLine(mensaje);
    }

    public static void MuestraEspecial(string mensaje)
    {
        Console.ForegroundColor = ColorEspecial;
        Console.WriteLine(mensaje);
    }
}

class Program
{
    static void Main(string[] args)
    {
        MyScraper scraper = new MyScraper();
        ContructorDePdf constructor = new ContructorDePdf();
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

        int capitulosPorPdf = 50;
        string Path = "C:\\Users\\Juan\\Desktop\\Novelas\\";



        Mensajero.MuestraNotificacion(  $"Titulo: {Titulo}\n" +
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
            "//div[@class = 'text-left']/p",
            "//div[@class = 'cha-words']/p",
            "//*[@class = 'desc']/p"
        };
        //---------------------------

        //Preparaciones
        constructor.InicializaConstructor(Titulo, capitulosPorPdf, Path);
        scraper.InicializaScrapper(xPaths);
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

    private static void MustraResultado(MyScraper scraper, ContructorDePdf constructor)
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


//iText7_Test(Test, "Novela X", 2, Path);