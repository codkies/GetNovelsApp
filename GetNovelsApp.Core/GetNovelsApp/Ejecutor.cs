using System;
using System.Collections.Generic;

using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.Core.Modelos;

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

        public void ActualizaEjecutor(Novela novelaNueva, Configuracion configNueva)
        {            
            //Actualizando referencias para la siguiente iteracion.            
            ActualizaReferencias(novelaNueva, configNueva);
        }


        /// <summary>
        /// Obteniendo informacion de las iteraciones anteriores.
        /// </summary>
        private void RecolectaInformacion()
        {
            if (Constructor == null | Scraper == null) return;
            DocumentosCreados += Constructor.DocumentosCreados;
            EntradasIgnoradas += Scraper.EntradasIgnoradas;
            CaracteresVistos += Scraper.CaracteresVistos;
            CapitulosEncontrados += Scraper.CapitulosEncontrados;
        }


        /// <summary>
        /// Actualizando referencias para la siguiente iteracion.  
        /// </summary>
        /// <param name="novelaNueva"></param>
        /// <param name="configNueva"></param>
        private void ActualizaReferencias(Novela novelaNueva, Configuracion configNueva)
        {
            NovelaActual = novelaNueva;
            ConfiguracionActual = configNueva;

            Titulo = novelaNueva.Titulo;
            link = novelaNueva.link;
            primerCap = novelaNueva.PrimerCap;
            ultimoCap = novelaNueva.UltimoCap;

            Path = configNueva.PathCarpeta;
            capitulosPorPdf = configNueva.CapitulosPorPdf;
            xPaths = configNueva.xPaths;

            Scraper = new Scraper(configNueva);
            Constructor = new PdfConstructor(NovelaActual, ConfiguracionActual);
        }

        #endregion


        #region Propieadades

        public int DocumentosCreados { get; private set; } = 0;

        public int EntradasIgnoradas { get; private set; } = 0;

        public long CaracteresVistos { get; private set; } = 0;

        public int CapitulosEncontrados { get; private set; } = 0;


        #endregion


        #region Fields

        public Scraper Scraper;

        public PdfConstructor Constructor;

        private Novela NovelaActual;

        private Configuracion ConfiguracionActual;

        string Titulo;
        string link;
        int ultimoCap;
        int primerCap;

        string Path;
        int capitulosPorPdf;

        List<string> xPaths = new List<string>(); 
        

        #endregion


        public void Ejecuta()
        {
            for (int indexCap = primerCap; indexCap <= ultimoCap; indexCap++)
            {
                Capitulo Capitulo = Scraper.ObtenCapitulo(link, indexCap);
                Constructor.AgregaCapitulo(Capitulo);

                //Meter este como "Especial" en el Mensajero.
                Mensajero.MuestraEspecial($"Program --> {indexCap}/{ultimoCap}");

                link = Scraper.SiguienteDireccion;

                bool HayOtroCapitulo = !link.Equals(string.Empty);
                if (!HayOtroCapitulo)
                {
                    Mensajero.MuestraNotificacion($"Program--> No se encontró otro capitulo. Capitulo {indexCap} fue el ultimo hallado.");
                    Constructor.FinalizoNovela();
                    break;
                }
            }
            Constructor.FinalizoNovela();
            RecolectaInformacion();
        }
    }
}
