using System;
using System.Collections.Generic;

using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.Core.Modelos;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using iText.Layout.Properties;

namespace GetNovelsApp.Core
{
    /// <summary>
    /// Encargado de ejecutar el programa en el orden adecuado.
    /// </summary>
    public class Ejecutor
    {
        #region Constructores & Setup

        public Ejecutor()
        {

        }

        /// <summary>
        /// Obteniendo informacion de las iteraciones anteriores.
        /// </summary>
        private void RecolectaInformacion()
        {
            if (ConstructorActual == null | ScraperActual == null) return;
            DocumentosCreados += ConstructorActual.DocumentosCreados;
            EntradasIgnoradas += ScraperActual.EntradasIgnoradas;
            CaracteresVistos += ScraperActual.CaracteresVistos;
            CapitulosEncontrados += ScraperActual.CapitulosEncontrados;
        }

        #endregion


        #region Propieadades

        public int DocumentosCreados { get; private set; } = 0;

        public int EntradasIgnoradas { get; private set; } = 0;

        public long CaracteresVistos { get; private set; } = 0;

        public int CapitulosEncontrados { get; private set; } = 0;

        public int Skips { get; private set; } = 0;


        #endregion


        #region Fields

        private Scraper ScraperActual;

        private PdfConstructor ConstructorActual;

        private Novela NovelaActual;


        /// <summary>
        /// Ultimo capitulo segun la novela.
        /// </summary>
        private float UltimoCapActual => NovelaActual.UltimoNumeroCapitulo;


        /// <summary>
        /// Primer capitulo segun la novela.
        /// </summary>
        private float PrimerCapActual => NovelaActual.PrimerNumeroCapitulo;




        #endregion


        #region Core

        public void Ejecuta(Novela novelaNueva)
        {
            //Actualizando referencias
            TomaReferencias(novelaNueva);
            //------------------------------------------

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Mensajero.MuestraEspecial($"\nEjecutor --> {NovelaActual.Titulo} tiene {UltimoCapActual} capitulos. Se empezará desde el link #{NovelaActual.EmpezarEn}");
            Mensajero.MuestraEspecial($"Ejecutor --> Se realizarán {NovelaActual.LinksDeCapitulos.Count - NovelaActual.EmpezarEn} iteraciones.");

            for (int i = NovelaActual.EmpezarEn; i < NovelaActual.LinksDeCapitulos.Count; i++)
            {
                string Link = NovelaActual.LinksDeCapitulos[i];

                //Core:
                Mensajero.MuestraNotificacion($"\nEjecutor --> Comenzando iteracion #{i}...");
                Capitulo Capitulo = ScrapCapitulo(Link);

                //Mensajes:
                Mensajero.MuestraEspecial($"Ejecutor --> {Capitulo.TituloCapitulo} obtenido.");
                Mensajero.MuestraExito($"Ejecutor --> Iteracion #{i}/{NovelaActual.LinksDeCapitulos.Count} completada.");
            }

            stopwatch.Stop();
            Mensajero.MuestraExito($"\nEjecutor --> Se han finalizado todas las iteraciones. Tiempo tomado: {stopwatch.ElapsedMilliseconds / 1000}s.");
            ConstructorActual.FinalizoNovela();
            RecolectaInformacion();
        }


        private Capitulo ScrapCapitulo(string Link)
        {
            Capitulo Capitulo = ScraperActual.ObtenCapitulo(Link);
            ConstructorActual.AgregaCapitulo(Capitulo);
            return Capitulo;
        }


        private void TomaReferencias(Novela novelaNueva)
        {
            NovelaActual = novelaNueva;

            ScraperActual = new Scraper();
            ConstructorActual = new PdfConstructor(NovelaActual);
        }

        #endregion

        /// <summary>
        /// Testing async.
        /// </summary>
        /// <param name="novelaNueva"></param>
        public async Task t_EjecutaAsync(Novela novelaNueva)
        {
            //Actualizando referencias
            TomaReferencias(novelaNueva);
            //------------------------------------------

            //Preparaciones:
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Mensajero.MuestraEspecial($"\nEjecutor --> {NovelaActual.Titulo} tiene {UltimoCapActual} capitulos. Se empezará desde el link #{NovelaActual.EmpezarEn}");
            Mensajero.MuestraEspecial($"Ejecutor --> Se realizarán {NovelaActual.LinksDeCapitulos.Count - NovelaActual.EmpezarEn} iteraciones.");
            //------------------------------------------------------------------------------------------------------------------------------


            //Scraping:
            Mensajero.MuestraNotificacion("Ejecutor --> Comenzando Scrap");
            List<Capitulo> ScrapedChapters = await ScrapCapitulosAsync(); //Depede de que este script tenga las referencias a la novela actual.
            Mensajero.MuestraNotificacion("Ejecutor --> Finalizado Scrap");

            //Imprimiendo:
            foreach (Capitulo cap in ScrapedChapters)
            {
                ConstructorActual.AgregaCapitulo(cap);
            }

            //Reportando:            
            stopwatch.Stop();
            Mensajero.MuestraExito($"\nEjecutor --> Se han finalizado todas las iteraciones. Tiempo tomado: {stopwatch.ElapsedMilliseconds / 1000}s.");
            ConstructorActual.FinalizoNovela();
            RecolectaInformacion();
        }

        private async Task<List<Capitulo>> ScrapCapitulosAsync()
        {
            List<Capitulo> ScrapedChapters = new List<Capitulo>();
            List<Task<Capitulo>> tareas = new List<Task<Capitulo>>();


            for (int i = NovelaActual.EmpezarEn; i < NovelaActual.LinksDeCapitulos.Count; i++)
            {
                string Link = NovelaActual.LinksDeCapitulos[i];                
                tareas.Add(Task.Run(() => ScraperActual.ObtenCapitulo(Link)));                
            }

            var resultados = await Task.WhenAll(tareas);

            List<Capitulo> capsDesordenados = new List<Capitulo>(resultados);
            
            capsDesordenados.Sort(new OrdenadorCapitulos());

            foreach (Capitulo cap in capsDesordenados)
            {
                ScrapedChapters.Add(cap);
            }
            
            return ScrapedChapters;
        }
    }


    public class OrdenadorCapitulos : Comparer<Capitulo>
    {
        public override int Compare(Capitulo x, Capitulo y)
        {
            if (x.NumeroCapitulo > y.NumeroCapitulo)
                return 1;
            else if (x.NumeroCapitulo < y.NumeroCapitulo)
                return -1;
            else
                throw new NotSupportedException("Ambos capitulos tienen el mismo numero");
        }
    }
}
