using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            string ver = "v0.10.0";
            Mensajero.MuestraEspecial($"GetAppsNovel {ver}\n    ... Check version before commiting.");

            PideInformacionUsuario(out List<Novela> Novelas);


            //List<Novela> Novelas = new List<Novela>()
            //{
            //    new Novela("https://wuxiaworld.site/novel/the-king-of-the-battlefield/", "C:\\Users\\Juan\\Desktop\\Novelas\\The King of the Battlefield")
            //};

            //Diagnostics:
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Core:
            Ejecutor ejecutor = new Ejecutor();
            foreach (Novela novela in Novelas)
            {
                Mensajero.MuestraEspecial($"Program --> Comenzando novela {novela.Titulo}");
                System.IO.Directory.CreateDirectory(novela.CarpetaPath);

                ejecutor.Ejecuta(novela);
                Mensajero.MuestraExito($"Program --> Terminando novela {novela.Titulo}");
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
        private static void PideInformacionUsuario(out List<Novela> Novelas)
        {
            Novelas = new List<Novela>();
            //Obteniendo informacion de novelas.  

            bool InputFinalizado = false;
            int numeroDeNovelas = 1;

            MuestraInput("Carpeta: (Se creará una subcarpeta con el titulo de cada novela).", out string Path);

            while (!InputFinalizado)
            {
                //Campos de input:                
                MuestraInput("Link de la página principal de la novelas:", out string LinkNovela);

                Mensajero.MuestraNotificacion("\nObteniendo información de novela...\n");

                //Arreglando informacion:
                Path = Path.Replace(@"\\", @"\\\\");
                Novela novela = new Novela(LinkNovela, Path);

                ///Confirmando con el usuario:
                Mensajero.MuestraNotificacion($"Titulo: {novela.Titulo}\n" +
                                            $"Link: {LinkNovela}\n" +
                                            $"Cantidad de capitulos: {novela.LinksDeCapitulos.Count}\n" +
                                            $"CapitulosPorPdf: {Configuracion.CapitulosPorPdf}\n" +
                                            $"Carpeta: {novela.CarpetaPath}");

                MuestraInput("\n Confirmar (Y/N)", out string respuesta, ColorTitulo: ConsoleColor.Red);


                if (respuesta.Equals("y") | respuesta.Equals("yes"))
                {
                    Novelas.Add(novela);
                    numeroDeNovelas++;
                    Mensajero.MuestraNotificacion($"Program --> Confirmada {novela.Titulo}");
                }
                else
                {
                    Mensajero.MuestraError($"Program --> Descartada {novela.Titulo}");
                }


                bool decisionTomada = false;
                while (!decisionTomada)
                {
                    string decision = Mensajero.TomaMensaje("Quieres agregar otra novela? (Y/N)");
                    if (decision.Equals("n") | decision.Equals("no") | decision.Equals("not"))
                    {
                        decisionTomada = true;
                        InputFinalizado = true;
                    }
                    else if (decision.Equals("y") | decision.Equals("yes") | decision.Equals("ye"))
                    {
                        decisionTomada = true;
                    }
                }

                if (!InputFinalizado) continue;
                Mensajero.MuestraNotificacion($"\nSe obtendrán {Novelas.Count} novelas:");
                for (int i = 0; i < Novelas.Count; i++)
                {
                    Novela n = Novelas[i];
                    Mensajero.MuestraNotificacion($"#{i + 1} {n.Titulo}");
                }
                Mensajero.TomaMensaje("\nPresiona cualquier tecla para comenzar.");
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

