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
        private void ActualizaReferencias(Novela novelaNueva, Configuracion configNueva)
        {
            NovelaActual = novelaNueva;
            ConfiguracionActual = configNueva;

            TituloActual = novelaNueva.Titulo;
            LinkActual = novelaNueva.PrimerLink;
            PrimerCapActual = novelaNueva.PrimerNumeroCapitulo;
            UltimoCapActual = novelaNueva.UltimoCap;

            PathActual = configNueva.PathCarpeta;
            CapitulosPorPDFActual = configNueva.CapitulosPorPdf;
            xPathsActual = configNueva.xPaths;

            ScraperActual = new Scraper(configNueva);
            ConstructorActual = new PdfConstructor(NovelaActual, ConfiguracionActual);
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

        private Configuracion ConfiguracionActual;

        string TituloActual;
        string LinkActual;
        int UltimoCapActual;
        int PrimerCapActual;

        string PathActual;
        int CapitulosPorPDFActual;

        List<string> xPathsActual = new List<string>(); 
        

        #endregion


        public void Ejecuta(Novela novelaNueva, Configuracion configNueva)
        {
            ActualizaReferencias(novelaNueva, configNueva);

            int CantidadIteraciones = UltimoCapActual - PrimerCapActual; //Cantidad de iteraciones

            Mensajero.MuestraEspecial($"\nEjecutor --> Buscando capitulos: {PrimerCapActual}-{UltimoCapActual}");
            Mensajero.MuestraEspecial($"Ejecutor --> Se realizarán {CantidadIteraciones} iteraciones.");
            for (int i = 1; i <= CantidadIteraciones; i++)
            {

                //Core:
                Mensajero.MuestraNotificacion($"Ejecutor --> Comenzando iteracion #{i}...");
                Capitulo Capitulo = ScraperActual.ObtenCapitulo(LinkActual);                
                ConstructorActual.AgregaCapitulo(Capitulo);

                //Cosas relacionadas al loop:
                if(Capitulo.Valor > 1)
                {
                    //Cambios:
                    int valorNuevo = CantidadIteraciones - (Capitulo.Valor - 1); //Valor nuevo de la CantidadIteraciones.
                    Skips += (Capitulo.Valor - 1);
                    Mensajero.MuestraCambioEstado($"Ejecutor --> El capitulo {Capitulo.TituloCapitulo} vale por {Capitulo.Valor}. Se reducirá la cantidad de iteraciones totales de {CantidadIteraciones} a {valorNuevo}.");
                    CantidadIteraciones = valorNuevo; //Itera 1 vez menos si el capitulo contaba como 2 capitulos.  
                }

                //Mensajes:
                Mensajero.MuestraEspecial($"Ejecutor --> Capitulo {Capitulo.NumeroCapitulo} obtenido.");
                Mensajero.MuestraExito($"Ejecutor --> Iteracion #{i} completada.");

                if(i < CantidadIteraciones)
                {
                    Mensajero.MuestraNotificacion($"\nEjecutor --> Buscando siguiente direccion...");
                    LinkActual = ScraperActual.SiguienteDireccion;

                    bool HayOtroCapitulo = !LinkActual.Equals(string.Empty);
                    if (!HayOtroCapitulo)
                    {
                        Mensajero.MuestraNotificacion($"Ejecutor--> No se encontró otro capitulo. Capitulo {i} fue el ultimo hallado.");
                        ConstructorActual.FinalizoNovela();
                        break;
                    }
                }
                else
                {
                    Mensajero.MuestraExito($"\nEjecutor --> Se han finalizado todas las iteraciones.");
                }
            }
            ConstructorActual.FinalizoNovela();
            RecolectaInformacion();
        }
    }
}
