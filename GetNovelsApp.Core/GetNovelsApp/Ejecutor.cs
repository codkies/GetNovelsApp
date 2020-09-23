using System;
using System.Collections.Generic;

using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.Core.Modelos;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Diagnostics;

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

        //Testing:

        public void t_Ejecuta(Novela novelaNueva)
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
            List<Capitulo> ScrapedChapters = new List<Capitulo>();            

            for (int i = NovelaActual.EmpezarEn; i < NovelaActual.LinksDeCapitulos.Count; i++)
            {
                string Link = NovelaActual.LinksDeCapitulos[i];
                
                Mensajero.MuestraNotificacion($"\nEjecutor --> Comenzando iteracion #{i}...");
                Capitulo Capitulo = ScraperActual.ObtenCapitulo(Link);
                ScrapedChapters.Add(Capitulo);
                Mensajero.MuestraExito($"Ejecutor --> Iteracion #{i}/{NovelaActual.LinksDeCapitulos.Count} completada.");
            }

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





    }
}
