using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

using GetNovelsApp.Core;
using GetNovelsApp.Core.Empaquetador;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.GetNovelsApp.Empaquetador.BaseDatos;
using GetNovelsApp.Core.Modelos;

namespace GetAppsNovel.ConsoleVersion
{
    class Program 
    {
        static ConfiguracionConsoleUI configuracion;
        static GetNovels getNovels;       
        static ConsoleUI ConsoleUI = new ConsoleUI();

        private static void SetupApp()
        {
            string ver = "v0.14.0";
            string message = "DB Sucks.\n" +
                                "   - Program funciona con info de novela, no con novelas completas.";
            ConsoleUI.ReportaEspecial($"GetNovelsApp {ver}:\n{message}", ConsoleUI);
            configuracion = ConsoleUI.PideConfiguracion();
            getNovels = new GetNovels(configuracion);
        }


        static async Task Main(string[] args)
        {
            var x = DataBaseAccess.GetConnectionString();
            var y = DataBaseAccess.GetConnection();

            //Ver control.            
            SetupApp();

            Dictionary<InformacionNovela, int> InfoNovelas = ConsoleUI.PideInformacionUsuario(configuracion.FolderPath);

            //Diagnostics:
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Core:            
            await RunProgram(InfoNovelas);

            //Diagnostics:
            stopwatch.Stop();
            ConsoleUI.MustraResultado(getNovels, stopwatch);
        }

        /// <summary>
        /// Core de este script
        /// </summary>
        /// <param name="InfoNovelas"></param>
        /// <returns></returns>
        private static async Task RunProgram(Dictionary<InformacionNovela, int> InfoDescargas)
        {
            foreach (KeyValuePair<InformacionNovela, int> item in InfoDescargas)
            {
                InformacionNovela infoNovela = item.Key;
                int ComienzaEn = item.Value;

                ConsoleUI.ReportaEspecial($"Comenzando novela \"{infoNovela.Titulo}\"", ConsoleUI);

                Novela novela = await getNovels.GetNovelAsync(infoNovela, ComienzaEn); //Hardcoeando aqui el pdf.

                ConsoleUI.ReportaEspecial($"Terminando novela \"{novela.Titulo}\"", ConsoleUI);
                
                //Preguntando al usuario si quiere imprimir la novela.
                bool decisionTomada = false;
                while (decisionTomada == false)
                {
                    string decision = ConsoleUI.PreguntaSiSeImprime(novela);
                    if (decision.Equals("y") | decision.Equals("yes"))
                    {
                        string Path = LocalPathManager.DefinePathNovela(novela);
                        System.IO.Directory.CreateDirectory(Path);
                        EventsManager.Invoke_ImprimeNovela(novela, TiposDocumentos.PDF); //Harcodeando el tipo de doc.
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


    }
}

