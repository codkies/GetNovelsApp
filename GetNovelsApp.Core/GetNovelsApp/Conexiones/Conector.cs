using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web.WebSockets;
using GetNovelsApp.Core.Utilidades;
using HtmlAgilityPack;

namespace GetNovelsApp.Core.Conexiones
{
    public class Conector
    {
        /// <summary>
        /// Construye un conector.
        /// </summary>
        /// <param name="tiempoTopeEnSegundos">Superado este tiempo, el conector dejará de intentar conectar.</param>
        public Conector(int tiempoTopeEnSegundos)
        {
            Conexion = new HtmlWeb();
            TiempoTopMilisegundos = tiempoTopeEnSegundos * 1000;
        }

        public HtmlWeb Conexion { get; private set; }

        public int TiempoTopMilisegundos { get; private set; }


        /// <summary>
        /// It tries hard to connect to some direction.
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="tiempoDeEspera"></param>
        /// <returns></returns>
        public HtmlDocument HardConnect(string direccion, int tiempoDeEspera = 5000)
        {
            Mensajero.MuestraNotificacion("Conector --> Comenzando conexión...");
            HtmlDocument doc = null;

            Stopwatch stopwatchTotal = new Stopwatch();

            stopwatchTotal.Start();
            while (doc == null & stopwatchTotal.ElapsedMilliseconds < TiempoTopMilisegundos)
            {
                try
                {
                    doc = Conexion.Load(direccion);
                }
                catch(TimeoutException)
                {
                    Mensajero.MuestraNotificacion("Conector --> Timeout. Reintentando...");
                    System.Threading.Thread.Sleep(tiempoDeEspera); //Wait for 5seconds 

                    if (stopwatchTotal.ElapsedMilliseconds > 60000)
                    {
                        Mensajero.MuestraError($"Han pasado {stopwatchTotal.ElapsedMilliseconds / 1000}s intentando conectar con {direccion}");
                    }
                }
                catch(System.Net.WebException)
                {
                    Mensajero.MuestraErrorMayor("No hay internet.");
                }
            }
            if(doc == null) Mensajero.MuestraErrorMayor("Conector --> Superado el tiempo de espera para obtener website.");
            else Mensajero.MuestraNotificacion("Conector --> Conexion establecida.");
            return doc;
        }



        public HtmlNodeCollection ObtenNodes(HtmlDocument doc, List<string> xPaths)
        {
            Mensajero.MuestraNotificacion("Conector --> Buscando nodes...");
            HtmlNodeCollection posibleColeccion = null;

            Stopwatch stopwatchTotal = new Stopwatch();
            stopwatchTotal.Start();

            foreach (string xPath in xPaths)
            {
                posibleColeccion = doc.DocumentNode.SelectNodes(xPath);
                if (posibleColeccion != null) break;
            }            

            if (posibleColeccion == null) Mensajero.MuestraErrorMayor("Conector --> Nodes no encontrados.");
            else Mensajero.MuestraNotificacion("Conector --> Nodes encontrados.");
            return posibleColeccion;
        }


        #region Shorthands

        public HtmlNodeCollection ObtenNodes(HtmlDocument doc, string xPath)
        {
            return ObtenNodes(doc, new List<string>() { xPath });
        }


        #endregion

    }
}
