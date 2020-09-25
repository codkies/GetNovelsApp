using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Configuracion;
using GetNovelsApp.Core.CreadorDocumentos;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;

namespace GetAppsNovel.ConsoleVersion
{
    class Program
    {
        static readonly ConsoleUI mensajero = new ConsoleUI();
        static readonly ConfiguracionBasica configuracion = new ConfiguracionBasica();
        static readonly GetNovels getNovels = new GetNovels(configuracion, mensajero);
        static readonly ComunicadorUsuario comunicador = new ComunicadorUsuario();

        //Fix entry point stuff.
        static async Task Main(string[] args)
        {
            //Ver control.            
            string ver = "v0.12.0"; //Orden de libreria.
            mensajero.ReportaEspecial($"GetAppsNovel {ver} ... Check version before commiting.", comunicador);

            //Pidiendo info al usuario:
            List<Novela> Novelas = comunicador.PideInformacionUsuario();

            //Diagnostics:
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            //Core:
            await PasaNovelasEjecutadorAsync(Novelas);

            //Diagnostics:
            stopwatch.Stop();
            comunicador.MustraResultado(getNovels, stopwatch);
        }

        private static async Task PasaNovelasEjecutadorAsync(List<Novela> Novelas)
        {
            foreach (Novela novela in Novelas)
            {
                mensajero.ReportaEspecial($"Comenzando novela \"{novela.Titulo}\"", comunicador);
                System.IO.Directory.CreateDirectory(novela.CarpetaPath);

                await getNovels.EjecutaAsync(novela, TiposDocumentos.PDF); //Hardcoeando aqui el pdf.
                mensajero.ReportaEspecial($"Terminando novela \"{novela.Titulo}\"", comunicador);
            }
        }


    }
}

