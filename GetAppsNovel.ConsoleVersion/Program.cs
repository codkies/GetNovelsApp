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
            string ver = "v0.8";
            Mensajero.MuestraEspecial($"GetAppsNovel {ver}\n... Check version before commiting.");

            List<string> xPaths = new List<string>()
            {
                "//div[@class = 'cha-words']/p",
                "//div[@class = 'text-left']/p",
                "//*[@class = 'desc']/p"
            };

            List<string> xPathsSiguienteBoton = new List<string>()
            {
                "//div[@class='nav-next']/a", //Wuxiaworldsite
                "//li/a[@class='next next-link']", //readlightnovels

            };


            PideInformacionUsuario(xPaths, xPathsSiguienteBoton, out Dictionary<Novela, Configuracion> Novelas);
            

            //Diagnostics:
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Core:
            Ejecutor ejecutor = new Ejecutor();
            foreach (KeyValuePair<Novela, Configuracion> entry  in Novelas)
            {
                Mensajero.MuestraEspecial($"Program --> Comenzando novela {entry.Key.Titulo}");
                System.IO.Directory.CreateDirectory(entry.Value.PathCarpeta);

                ejecutor.Ejecuta(entry.Key, entry.Value);
                Mensajero.MuestraExito($"Program --> Terminando novela {entry.Key.Titulo}");
            }

            //Diagnostics:
            stopwatch.Stop();
            MustraResultado(ejecutor, stopwatch);
        }



        #region Cosas de input de la consola


        /// <summary>
        /// Crea un formulario para que el usuario meta la informacion a buscar.
        /// </summary>
        /// <param name="xPaths"></param>
        /// <param name="Novelas"></param>
        private static void PideInformacionUsuario(List<string> xPaths, List<string> xPathsSiguienteBoton, out Dictionary<Novela, Configuracion> Novelas)
        {
            Novelas = new Dictionary<Novela, Configuracion>();
            //Obteniendo informacion de novelas.  
            MuestraInput("\nCantidad de novelas:", out string cantidadNovelas);
            int CantidadNovelas = int.Parse(cantidadNovelas);
            //Inputs:
            for (int i = 1; i <= CantidadNovelas; i++)
            {
                //Campos de input:
                MuestraInput($"\nTitulo de la novela #{i}:", out string Titulo);
                MuestraInput("Link del primer capitulo:", out string link);
                MuestraInput("Ultimo capitulo:", out string _ultimoCap);
                MuestraInput("Capitulos por PDF:", out string capsPorPDF);
                MuestraInput("Carpeta: (Se creará una subcarpeta con el titulo de la novela)", out string Path);               


                ///Confirmando con el usuario:
                Mensajero.MuestraNotificacion($"Titulo: {Titulo}\n" +
                                            $"Link: {link}\n" +
                                            $"UltimoCap: {_ultimoCap}\n" +
                                            $"CapitulosPorPdf: {capsPorPDF}\n" +
                                            $"Carpeta: {Path}");

                MuestraInput("\n... Y para confirmar. Enter para repetir.", out string respuesta, ColorTitulo: ConsoleColor.Red);

                if (respuesta.Equals("y"))
                {
                    //Manipulado la info del usuario:
                    int capitulosPorPdf = int.Parse(capsPorPDF);
                    int ultimoCap = int.Parse(_ultimoCap);
                    Path = Path.Replace(@"\\", @"\\\\");
                    Path += $"\\{Titulo}\\";

                    ///Creando Structs:
                    Novela novela = new Novela(Titulo, link, ultimoCap);

                    Configuracion configuracion = new Configuracion(xPaths, xPathsSiguienteBoton, Path, capitulosPorPdf);
                    //Agregando el 
                    Novelas.Add(novela, configuracion);
                    Mensajero.MuestraNotificacion($"Program --> Confirmada {Titulo}");
                }
                else
                {
                    CantidadNovelas++;
                    Mensajero.MuestraError($"Program --> Descartada {Titulo}");
                    i--;                    
                }                
            }
        }


        private static void MuestraInput(string titulo, out string Obten, ConsoleColor ColorTitulo = ConsoleColor.Cyan, ConsoleColor ColorEscrito = ConsoleColor.White)
        {
            Console.ForegroundColor = ColorTitulo;
            Console.WriteLine(titulo);
            Console.ForegroundColor = ColorEscrito;
            Obten = Console.ReadLine();            
        }

        private static void MustraResultado(Ejecutor ejecutor, Stopwatch stopwatch)
        {
            TimeSpan ts = stopwatch.Elapsed;
            string report = ts.ToString("hh\\:mm\\:ss\\.ff");

            Mensajero.MuestraEspecial($"Finalizado el proceso en {report}. Resumen: \n" +
                        $"  Se han creado {ejecutor.DocumentosCreados} documentos.\n" +
                        $"  Se han ignorado {ejecutor.EntradasIgnoradas} entradas.\n" +
                        $"  Se han saltado {ejecutor.Skips} iteraciones.\n" +
                        $"  Se han obtenido {ejecutor.CapitulosEncontrados} capitulos.\n" +
                        $"  Para un total de {ejecutor.CaracteresVistos} caracteres.");

            Mensajero.MuestraExito("press Enter to exit.");
            Console.ReadLine();
        }

        #endregion
    }
}

