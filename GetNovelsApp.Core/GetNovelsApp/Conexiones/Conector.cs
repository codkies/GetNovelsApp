using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web.WebSockets;
using GetNovelsApp.Core.Utilidades;
using HtmlAgilityPack;
using iText.Kernel.Pdf.Canvas.Parser.ClipperLib;

namespace GetNovelsApp.Core.Conexiones
{
    public class Conector
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
                    Mensajero.MuestraError("Conector --> No se consiguieron los nodos segun los xPaths. Presiona enter para reintentar.");
                    Console.ReadLine();
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
                nodosOne = ObtenNodes(website, xPathsOne); //Check for null.
                nodosTwo = ObtenNodes(website, xPathsTwo); //Check for null.

                if (nodosOne == null | nodosTwo == null)
                {
                    Mensajero.MuestraError("Conector --> No se consiguieron los nodos segun los xPaths. Presiona enter para reintentar.");
                    Console.ReadLine();
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
                        Debug.WriteLine("Conector --> Error. \n" +
                                        $"Direccion: {direccion} \n" +
                                        $"xPaths: {xPaths}");
                        break;
                    }

                    AllHtmlNodes.Add(posiblesNodos);
                }

                if (AllHtmlNodes == null)
                {
                    Mensajero.MuestraError("Conector --> No se consiguieron los nodos segun los xPaths. Presiona enter para reintentar.");
                    Console.ReadLine();
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
            HtmlDocument doc = null;

            while (doc == null)
            {
                try
                {
                    doc = Conexion.Load(direccion);
                }
                catch (TimeoutException)
                {
                    Mensajero.MuestraNotificacion("Conector --> Timeout. Reintentando...");
                    System.Threading.Thread.Sleep(tiempoDeEspera); //Wait for 5seconds                     
                }
                catch (WebException)
                {
                    Mensajero.MuestraError("Conector --> Pareces no tener internet. Presiona enter para reintentarlo.");
                    Console.ReadLine();
                    continue;
                }
            }
            return doc;
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
