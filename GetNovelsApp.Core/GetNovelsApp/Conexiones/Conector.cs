using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using GetNovelsApp.Core.Reportaje;
using HtmlAgilityPack;

namespace GetNovelsApp.Core.Conexiones
{
    public class Conector : IReportero
    {
        #region Ctors

        /// <summary>
        /// Construye un conector.
        /// </summary>
        /// <param name="tiempoTopeEnSegundos">Superado este tiempo, el conector dejará de intentar conectar.</param>
        public Conector(int tiempoTopeEnSegundos)
        {
            Conexion = new HtmlWeb();
            TiempoTopMilisegundos = tiempoTopeEnSegundos * 1000;
        }

        #endregion

        #region Fields

        private HtmlWeb Conexion;

        private int TiempoTopMilisegundos;

        public string Nombre => "Conector";

        #endregion


        #region Publicos

        /// <summary>
        /// Regresa nodos de un website acorde a los xPaths.
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="xPaths"></param>
        /// <returns></returns>
        public HtmlNodeCollection IntentaNodos(string direccion, List<string> xPaths)
        {
            HtmlDocument website = ObtenWebsite(direccion);

            HtmlNodeCollection nodos = null;

            while (nodos == null)
            {
                nodos = ObtenNodes(website, xPaths); //Check for null.
                if (nodos == null)
                {
                    AppGlobalMensajero.ReportaError("No se consiguieron los nodos segun los xPaths. Reintentando...", this);
                    website = null;
                    website = ObtenWebsite(direccion);
                }
            }

            return nodos;
        }


        /// <summary>
        /// Regresa un Array de HtmlNodeCollection del website al que lleva la dirección. Index 0 corresponderá a los nodos del xPathsOne. Index 1 corresponderá a los nodos del xPathsTwo.
        /// </summary>
        /// <param name="direccion">Link al website.</param>
        /// <param name="xPathsOne">xPaths de los primeros nodos.</param>
        /// <param name="xPathsTwo">xPaths de los segundos nodos.</param>
        /// <returns></returns>
        public HtmlNodeCollection[] IntenaVariosNodos(string direccion, List<string> xPathsOne, List<string> xPathsTwo)
        {
            HtmlNodeCollection[] htmlNodes = new HtmlNodeCollection[2];

            HtmlDocument website = ObtenWebsite(direccion);

            HtmlNodeCollection nodosOne = null;
            HtmlNodeCollection nodosTwo = null;

            while (nodosOne == null | nodosTwo == null)
            {
                if(nodosOne == null) nodosOne = ObtenNodes(website, xPathsOne); //Check for null.
                if (nodosTwo == null) nodosTwo = ObtenNodes(website, xPathsTwo); //Check for null.

                if (nodosOne == null | nodosTwo == null)
                {
                    AppGlobalMensajero.ReportaError("No se consiguieron los nodos segun los xPaths. Reintentando...", this);
                    website = ObtenWebsite(direccion);
                }
            }

            htmlNodes[0] = nodosOne;
            htmlNodes[1] = nodosTwo;

            return htmlNodes;
        }


        /// <summary>
        /// Regresa una Lista de HtmlNodeCollection. El index de cada xPath corresponde al index de sus nodos en la lista.
        /// </summary>
        /// <param name="direccion">Link al website.</param>
        /// <param name="ListOfxPaths">Lista de xPaths a conseguir en el website.</param>
        /// <returns></returns>
        public List<HtmlNodeCollection> IntenaVariosNodos(string direccion, List<List<string>> ListOfxPaths)
        {
            HtmlDocument website = ObtenWebsite(direccion);

            List<HtmlNodeCollection> AllHtmlNodes = null;            

            while (AllHtmlNodes == null)
            {
                foreach (List<string> xPaths in ListOfxPaths)
                {
                    HtmlNodeCollection posiblesNodos = ObtenNodes(website, xPaths);

                    if(posiblesNodos == null) //Consiguelos todos o ninguno.
                    {
                        AllHtmlNodes = null;
                        Debug.WriteLine("Error. \n" +
                                        $"Direccion: {direccion} \n" +
                                        $"xPaths: {xPaths}", this);
                        break;
                    }

                    AllHtmlNodes.Add(posiblesNodos);
                }

                if (AllHtmlNodes == null)
                {
                    AppGlobalMensajero.ReportaError("No se consiguieron los nodos segun los xPaths. Reintentando...", this);
                    website = ObtenWebsite(direccion);
                }
            }

            return AllHtmlNodes;
        }


        #endregion


        #region Privados


        /// <summary>
        /// It tries hard to connect to some direction.
        /// </summary>
        /// <param name="direccion"></param>
        /// <param name="tiempoDeEspera"></param>
        /// <returns></returns>
        private HtmlDocument ObtenWebsite(string direccion, int tiempoDeEspera = 5000)
        {
            //Mensajero.MuestraNotificacion("Conector --> Comenzando conexión...");
            HtmlWeb Conexion = new HtmlWeb();
            HtmlDocument website = null;

            while (website == null)
            { 
                try
                {
                    website = Conexion.Load(direccion);
                }
                catch (ObjectDisposedException)
                {
                    AppGlobalMensajero.ReportaError("System.ObjectDisposedException", this);
                    Conexion = new HtmlWeb();
                    website = null;
                }
                catch (TimeoutException)
                {
                    AppGlobalMensajero.Reporta("Timeout. Reintentando...", this);
                    website = null;
                    System.Threading.Thread.Sleep(tiempoDeEspera); //Wait for 5seconds                    
                }
                catch (WebException)
                {
                    AppGlobalMensajero.ReportaError("Pareces no tener internet. Reintentando...", this);
                    website = null;
                    continue;
                }
            }
            return website;
        }


        private HtmlNodeCollection ObtenNodes(HtmlDocument doc, List<string> xPaths)
        {
            //Mensajero.MuestraNotificacion("Conector --> Buscando nodes...");
            HtmlNodeCollection posibleColeccion = null;

            Stopwatch stopwatchTotal = new Stopwatch();
            stopwatchTotal.Start();

            foreach (string xPath in xPaths)
            {
                posibleColeccion = doc.DocumentNode.SelectNodes(xPath);
                if (posibleColeccion != null) break;
            }

            return posibleColeccion;
        }

        #endregion


    }
}
