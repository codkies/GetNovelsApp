using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices.WindowsRuntime;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Utilidades;

namespace GetAppsNovel.ConsoleVersion
{

    class Program
    {
        //Fix entry point stuff.
        static void Main(string[] args)
        {
            //Ver control.            
            Mensajero.MuestraEspecial($"GetAppsNovel v{Environment.Version}. Update version before commiting.");

            List<string> xPaths = new List<string>()
            {
                "//div[@class = 'cha-words']/p",
                "//div[@class = 'text-left']/p",
                "//*[@class = 'desc']/p"
            };

            Dictionary<Novela, Configuracion> Novelas = new Dictionary<Novela, Configuracion>();

            //Do not delete:
            {
                //Obteniendo informacion de novelas.  
                MuestraInput("Cantidad de novelas:", out string cantidadNovelas);
                int CantidadNovelas = int.Parse(cantidadNovelas);
                Inputs:
                for (int i = 1; i <= CantidadNovelas; i++)
                {
                    MuestraInput($"Titulo de la novela #{i}:", out string Titulo);
                    MuestraInput("Link del primer capitulo:", out string link);
                    MuestraInput("Primer capitulo:", out string _primerCap);
                    MuestraInput("Ultimo capitulo:", out string _ultimoCap);
                    MuestraInput("Capitulos por PDF:", out string capsPorPDF);
                    MuestraInput("Carpeta: (Se creará una subcarpeta con el titulo de la novela)", out string Path);
                    int capitulosPorPdf = int.Parse(capsPorPDF);
                    int primerCap = int.Parse(_primerCap);
                    int ultimoCap = int.Parse(_ultimoCap);

                    Path = Path.Replace(@"\\", @"\\\\");
                    Path += $"\\{Titulo}\\";
                    Novela novela = new Novela(Titulo, link, ultimoCap, primerCap);
                    Configuracion configuracion = new Configuracion(xPaths, Path, capitulosPorPdf);

                    Mensajero.MuestraNotificacion($"Titulo: {Titulo}\n" +
                                                $"Link: {link}\n" +
                                                $"PrimerCap: {primerCap}\n" +
                                                $"UltimoCap: {ultimoCap}\n" +
                                                $"CapitulosPorPdf: {capitulosPorPdf}\n" +
                                                $"Carpeta: {Path}");
                    MuestraInput("\n... Presiona enter para confirmar.", out string _, ColorTitulo: ConsoleColor.Red);

                    Novelas.Add(novela, configuracion);
                }
            }
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            Ejecutor ejecutor = new Ejecutor();
            foreach (KeyValuePair<Novela, Configuracion> entry  in Novelas)
            {
                Mensajero.MuestraEspecial($"Program --> Comenzando novela {entry.Key.Titulo}");
                System.IO.Directory.CreateDirectory(entry.Value.PathCarpeta);
                ejecutor.ActualizaEjecutor(entry.Key, entry.Value);
                ejecutor.Ejecuta();
            }            

            stopwatch.Stop();
            TimeSpan elapsed_time = stopwatch.Elapsed;

            MustraResultado(ejecutor, elapsed_time);
        }        

        #region Cosas de input de la consola

        private static void MuestraInput(string titulo, out string Obten, ConsoleColor ColorTitulo = ConsoleColor.Cyan, ConsoleColor ColorEscrito = ConsoleColor.White)
        {
            Console.ForegroundColor = ColorTitulo;
            Console.WriteLine(titulo);
            Console.ForegroundColor = ColorEscrito;
            Obten = Console.ReadLine();
            Console.WriteLine("\b");
        }

        private static void MustraResultado(Ejecutor ejecutor, TimeSpan elapsed_time)
        {
            Mensajero.MuestraExito($"Finalizado el proceso en {elapsed_time}. Resumen: \n" +
                        $"  Se han creado {ejecutor.DocumentosCreados}.\n" +
                        $"  Se han ignorado {ejecutor.EntradasIgnoradas} entradas.\n" +
                        $"  Se han obtenido {ejecutor.CapitulosEncontrados}.\n" +
                        $"  Total de {ejecutor.CaracteresVistos}.");

            Console.WriteLine("press Enter to exit.");
            Console.ReadLine();
        }

        #endregion
    }
}


class Legacy
{
    private static Dictionary<Novela, Configuracion> CreaEntradasEspeciales(List<string> xPaths)
    {
        Configuracion conf1 = new Configuracion(xPaths, "C:\\Novelas\\OEM\\", 100);

        Novela novela1 = new Novela("Otherworldly Evil Monarch",
            "https://wuxiaworld.site/novel/otherworldly-evil-monarch/chapter-700",
            1277,
            700);

        Configuracion conf2 = new Configuracion(xPaths, "C:\\Novelas\\ORV\\", 100);

        Novela novela2 = new Novela("Omniscient Reader’s Viewpoint",
            "https://wuxiaworld.site/novel/omniscient-readers-viewpoint/chapter-1",
            551,
            1);

        Configuracion conf3 = new Configuracion(xPaths, "C:\\Novelas\\LHP\\", 100);
        Novela novela3 = new Novela("Library of Heaven’s Path",
            "https://wuxiaworld.site/novel/library-of-heavens-path-novel/chapter-1",
            2268,
            1);

        Configuracion conf4 = new Configuracion(xPaths, "C:\\Novelas\\KoG\\", 150);
        Novela novela4 = new Novela("King of Gods",
            "https://wuxiaworld.site/novel/king-of-gods/chapter-1",
            1585,
            1);

        Configuracion conf5 = new Configuracion(xPaths, "C:\\Novelas\\OiLU\\", 270);
        Novela novela5 = new Novela("Only I level up",
            "https://wuxiaworld.site/novel/only-i-level-up-novel-solo-leveling-completed/chapter-1",
            270,
            1);

        Dictionary<Novela, Configuracion> Novelas = new Dictionary<Novela, Configuracion>
                {
                    { novela1, conf1 },
                    { novela2, conf2 },
                    { novela3, conf3 },
                    { novela4, conf4 },
                    { novela5, conf5 }
                };
        return Novelas;
    }
}