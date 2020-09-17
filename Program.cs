﻿using System;
using System.Collections.Generic;

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
        MuestraInput("Titulo:", out string Titulo);        
        MuestraInput("Link del primer capitulo:", out string link);
        MuestraInput("Primer capitulo:", out string primerCap);
        MuestraInput("Ultimo capitulo:", out string ultimoCap);
        MuestraInput("Capitulos por PDF:", out string capsPorPDF);
        MuestraInput("Carpeta:", out string Path);
        MuestraInput("\n... Presiona enter para confirmar.", out string _, ColorTitulo: ConsoleColor.Red);

        int capitulosPorPdf = int.Parse(capsPorPDF);
        int empiezaEn = int.Parse(primerCap);
        int terminaEn = int.Parse(ultimoCap);

        //string Titulo = "Versatile Mage";
        //string link = "https://www.readlightnovel.org/versatile-mage/chapter-1";      
        //int empiezaEn = 1;
        //int terminaEn = 1500;
        //int capitulosPorPdf = 5;
        //string Path = "C:\\Users\\Juan\\Desktop\\Novelas\\"
        //---------------------------

        //Configuracion de Scraper
        string xPath = "//*[@class = 'desc']/p";
        //---------------------------

        //Preparaciones
        constructor.InicializaConstructor(Titulo, capitulosPorPdf, Path);
        scraper.InicializaScrapper(xPath);
        //---------------------------       

        for (int i = 0; i < Test.Count; i++)
        {
            constructor.AgregaCapitulo(Test[i]);
        }
        constructor.FinalizoNovela();

        //for (int i = empiezaEn; i < terminaEn + 1; i++)
        //{
        //    string Capitulo = scraper.ObtenCapitulo(link, i);
        //    Console.WriteLine($"Program --> {i} capitulo obtenido. ///");
        //    constructor.AgregaCapitulo(Capitulo);

        //    if (scraper.HayOtroCapitulo)
        //    {
        //        link = scraper.SiguienteCapitulo;
        //    }
        //    else
        //    {
        //        constructor.FinalizoNovela();
        //        Console.WriteLine($"Program--> Ultimo capitulo: {i}. !!!");
        //        break;
        //    }
        //}

        //MustraResultado(scraper, constructor);
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


//iText7_Test(Test, "Novela X", 2, Path);