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


        /// <summary>
        /// Actualizando referencias para la siguiente iteracion.  
        /// </summary>
        /// <param name="novelaNueva"></param>
        /// <param name="configNueva"></param>
        private void ActualizaReferencias(Novela novelaNueva)
        {
            NovelaActual = novelaNueva;

            TituloActual = novelaNueva.Titulo;
            LinkActual = novelaNueva.PrimerLink;
            PrimerCapActual = novelaNueva.PrimerNumeroCapitulo;
            UltimoCapActual = novelaNueva.UltimoNumeroCapitulo;

            PathActual = novelaNueva.CarpetaPath;

            ScraperActual = new Scraper();
            ConstructorActual = new PdfConstructor(NovelaActual);
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

        public Scraper ScraperActual;

        public PdfConstructor ConstructorActual;

        private Novela NovelaActual;


        string TituloActual;
        string LinkActual;
        int UltimoCapActual;
        int PrimerCapActual;

        string PathActual;
        int CapitulosPorPDFActual;

        List<string> xPathsActual = new List<string>(); 
        

        #endregion


        public void Ejecuta(Novela novelaNueva)
        {
            ActualizaReferencias(novelaNueva);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            Mensajero.MuestraEspecial($"\nEjecutor --> Buscando capitulos: {PrimerCapActual}-{UltimoCapActual}");
            Mensajero.MuestraEspecial($"Ejecutor --> Se realizarán {NovelaActual.LinksDeCapitulos.Count} iteraciones.");

            for (int i = 1; i <= NovelaActual.LinksDeCapitulos.Count; i++)
            {
                string Link = NovelaActual.LinksDeCapitulos[i];

                //Core:
                Mensajero.MuestraNotificacion($"\nEjecutor --> Comenzando iteracion #{i}...");
                Capitulo Capitulo = ScraperActual.ObtenCapitulo(Link);
                ConstructorActual.AgregaCapitulo(Capitulo);                

                //Mensajes:
                Mensajero.MuestraEspecial($"Ejecutor --> {Capitulo.TituloCapitulo} obtenido.");
                Mensajero.MuestraExito($"Ejecutor --> Iteracion #{i} completada.");
            }

            stopwatch.Stop();
            Mensajero.MuestraExito($"\nEjecutor --> Se han finalizado todas las iteraciones. Tiempo tomado: {stopwatch.ElapsedMilliseconds/1000}s.");
            ConstructorActual.FinalizoNovela();
            RecolectaInformacion();
        }
    }
}
