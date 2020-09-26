using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Threading.Tasks;

using GetNovelsApp.Core;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Modelos;

namespace GetAppsNovel.ConsoleVersion
{
    class Program
    {        
        static readonly ConfiguracionConsoleUI configuracion = new ConfiguracionConsoleUI();
        static readonly GetNovels getNovels = new GetNovels(configuracion);        

        //Fix entry point stuff.
        static async Task Main(string[] args)
        {
            //Ver control.            
            string ver = "v0.13.0";
            string message = "Interfaces everywhere & DB.";
            configuracion.ConsoleUI.ReportaEspecial($"    GetNovelsApp {ver}: {message}", configuracion.ConsoleUI);

            //Pidiendo info al usuario:
            List<Novela> Novelas = configuracion.ConsoleUI.PideInformacionUsuario();

            //Diagnostics:
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Core:
            await PasaNovelasEjecutadorAsync(Novelas);

            //Diagnostics:
            stopwatch.Stop();
            configuracion.ConsoleUI.MustraResultado(getNovels, stopwatch);            
        }

        private static async Task PasaNovelasEjecutadorAsync(List<Novela> Novelas)
        {
            foreach (Novela novela in Novelas)
            {
                configuracion.ConsoleUI.ReportaEspecial($"Comenzando novela \"{novela.Titulo}\"", configuracion.ConsoleUI);
                System.IO.Directory.CreateDirectory(novela.CarpetaPath);

                await getNovels.EjecutaAsync(novela, TiposDocumentos.PDF); //Hardcoeando aqui el pdf.
                configuracion.ConsoleUI.ReportaEspecial($"Terminando novela \"{novela.Titulo}\"", configuracion.ConsoleUI);
                
                //Preguntando al usuario si quiere imprimir la novela.
                bool decisionTomada = false;
                while (decisionTomada == false)
                {
                    string decision = configuracion.ConsoleUI.PreguntaSiSeImprime(novela);
                    if (decision.Equals("y") | decision.Equals("yes"))
                    {
                        EventsManager.Invoke_ImprimeNovela(novela, TiposDocumentos.PDF);
                        decisionTomada = true;
                    }
                    else if (decision.Equals("n") | decision.Equals("no"))
                    {
                        configuracion.ConsoleUI.ReportaEspecial($"La novela \"{novela.Titulo}\"se ha guardado en la base de datos.",
                            configuracion.ConsoleUI);
                        decisionTomada = true;
                    }
                }
            }
        }


    }
}

