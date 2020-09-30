using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Empaquetador;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Modelos;

namespace GetAppsNovel.ConsoleVersion
{
    class Program 
    {
        static async Task Main(string[] args)
        {
            await NormalRun();
            //await TXTrun();
        }

        #region TXT run


        private static async Task TXTrun()
        {
            //Ver control.            
            SetupApp();

            Dictionary<INovela, int> InfoNovelas = ConsoleUI.PidePathTXTusuario(configuracion.FolderPath);

            //Diagnostics:
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Core:            
            await RunProgram(InfoNovelas);

            //Diagnostics:
            stopwatch.Stop();
            ConsoleUI.MustraResultado(getNovels, stopwatch);
        }


        #endregion

        #region Normal run

        static ConfiguracionConsoleUI configuracion;
        static GetNovels getNovels;
        static ConsoleUI ConsoleUI = new ConsoleUI();

        private static async Task NormalRun()
        {
            //Ver control.            
            SetupApp();

            Dictionary<INovela, int> InfoNovelas = ConsoleUI.PideInfoUsuario(configuracion.FolderPath);

            //Diagnostics:
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Core:            
            await RunProgram(InfoNovelas);

            //Diagnostics:
            stopwatch.Stop();
            ConsoleUI.MustraResultado(getNovels, stopwatch);
        }

        private static void SetupApp()
        {
            string ver = "v0.16";
            string message = "Last before WPF\n";
            ConsoleUI.ReportaEspecial($"GetNovelsApp {ver}:\n{message}", ConsoleUI);

            configuracion = ConsoleUI.PideConfiguracion();

            getNovels = new GetNovels(configuracion);
        }


        /// <summary>
        /// Core de este script
        /// </summary>
        /// <param name="InfoNovelas"></param>
        /// <returns></returns>
        private static async Task RunProgram(Dictionary<INovela, int> InfoDescargas)
        {
            foreach (KeyValuePair<INovela, int> item in InfoDescargas)
            {
                INovela novelaRT = item.Key;
                int ComienzaEn = item.Value;

                ConsoleUI.ReportaEspecial($"Buscando \"{novelaRT.Titulo}\"...", ConsoleUI);

                INovela novela = await getNovels.GetNovelAsync(novelaRT, ComienzaEn); //Hardcoeando aqui el pdf.

                ConsoleUI.ReportaEspecial($"Encontrada novela \"{novela.Titulo}\".", ConsoleUI);

                //Preguntando al usuario si quiere imprimir la novela.
                bool decisionTomada = false;
                while (decisionTomada == false)
                {
                    string decision = ConsoleUI.PreguntaSiSeImprime(novela);
                    if (decision.Equals("y") | decision.Equals("yes"))
                    {
                        string Path = LocalPathManager.DefinePathNovela(novela);
                        System.IO.Directory.CreateDirectory(Path);
                        GetNovelsEvents.Invoke_ImprimeNovela(novela, TiposDocumentos.PDF); //Harcodeando el tipo de doc.
                        decisionTomada = true;
                    }
                    else if (decision.Equals("n") | decision.Equals("no"))
                    {
                        ConsoleUI.ReportaEspecial($"La novela \"{novela.Titulo}\"se ha guardado en la base de datos.", ConsoleUI);
                        decisionTomada = true;
                    }
                }
            }
        }

        #endregion
        
    }
}

