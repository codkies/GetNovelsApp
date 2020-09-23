﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Web;
using GetNovelsApp.Core.Conexiones;
using GetNovelsApp.Core.Modelos;
using HtmlAgilityPack;

namespace GetNovelsApp.Core.Utilidades
{

    public static class ManipuladorDeLinks
    {
        /// <summary>
        /// Encuentra el numero del capitulo según el link.
        /// </summary>
        /// <param name="LinkCapitulo"></param>
        /// <returns></returns>
        public static InformacionCapitulo EncuentraInformacionCapitulo(string LinkCapitulo)
        {
            string numCapEscrito = string.Empty; //Para llevar record de numeros en formato de string. Luego se convierte a float.
            string TituloCapitulo = string.Empty; //Titulo del cap.
            int gruposDeNumeros = 0; //Grupos de numeros en el Link. ie: website.com/10-12/ ->> tiene 2 grupos de numeros.

            for (int i = 0; i < LinkCapitulo.Length; i++)
            {
                //Preparations.
                char letra = LinkCapitulo[i];
                bool EsUnNumero = char.IsDigit(letra);

                if (gruposDeNumeros == 1 & letra.Equals('-'))
                {
                    //Si hay un guión justo despues del primer grupo de numeros, conviertelo a un punto.
                    letra = '.'; 
                }
                else if (!EsUnNumero) continue; //Si no es un número, ignoralo.

                gruposDeNumeros++; //Paso, así que hay un grupo de numeros nuevo.

                //Haciendo un check de que hayan mas caracteres
                if (i == LinkCapitulo.Length - 1)
                {
                    numCapEscrito += letra.ToString();
                    TituloCapitulo += letra.ToString();
                    break;
                }
                //--------------------------------------------------------------

                //Core:
                numCapEscrito += letra.ToString(); //1  

                //Revisando las siguientes iteraciones:
                int salto = i + 1;
                bool subio = false;
                for (int x = salto; x < LinkCapitulo.Length; x++)
                {
                    char letraFutura = LinkCapitulo[x];
                    if (char.IsDigit(letraFutura)) //Solo procede si el caracter es un #
                    {
                        numCapEscrito += letraFutura.ToString();//2     
                        salto = x;
                        subio = true;
                    }
                    else break;//Apenas halles una letra, rompe este loop.
                }
                TituloCapitulo = numCapEscrito;
                i = subio ? salto : i; //Si el valor subió, has que la siguiente iteracion continue ahí. Sino, deja que continue normalmente.
            }

            float NumeroCapitulo;
            int Valor;

            //Posibles errores:
            if (gruposDeNumeros < 1)
            {
                Mensajero.MuestraError("No se pudo determinar el valor del capitulo.");
                Mensajero.MuestraCambioEstado($"La dirección es {LinkCapitulo}.");

                string inputUserTitulo = string.Empty;
                string inputUserNumCap = string.Empty;
                string inputUserValorCap = string.Empty;

                bool decisionTomada = false;
                while (!decisionTomada)
                {
                    inputUserTitulo = Mensajero.TomaMensaje($"\nEscribe el titulo del capitulo: (En general es 'Chapter - (numeroCapitulo)')");
                    inputUserNumCap = Mensajero.TomaMensaje($"Escribe el numero del capitulo: (puede tener decimales pero no acepta letras):");
                    inputUserValorCap = Mensajero.TomaMensaje($"Escribe por cuantos capitulos vale este: (si es un solo cap, el valor es 1. No acepta decimales ni letras):");

                    Mensajero.MuestraCambioEstado("Has escrito:\n" +
                                                    $"Direccion: {LinkCapitulo}\n" +
                                                    $"Titulo cap: {inputUserTitulo}\n" +
                                                    $"Numero del capitulo: {inputUserNumCap}\n" +
                                                    $"Valor del capitulo: {inputUserValorCap}");
                    string decision = Mensajero.TomaMensaje("Presiona (Y) para confirmar. Cualquier otra tecla para repetir.");

                    if(decision.Equals("y") | decision.Equals("yes"))
                    {
                        decisionTomada = true;
                    }  
                }
               

                NumeroCapitulo = Math.Abs(float.Parse(inputUserNumCap));
                Valor = Math.Abs(int.Parse(inputUserValorCap));
                TituloCapitulo = inputUserTitulo;
            }
            else
            {
                NumeroCapitulo = Math.Abs(float.Parse(numCapEscrito));
                Valor = gruposDeNumeros;
                TituloCapitulo = $"Chapter {TituloCapitulo}";
            }

            InformacionCapitulo infoDelCapituloSegunElLink = new InformacionCapitulo(TituloCapitulo, Valor, NumeroCapitulo);

            return infoDelCapituloSegunElLink;
        }


        public static InformacionNovela EncuentraInformacionNovela(string LinkPaginaPrincipal)
        {
            //Conexiones:
            Conector conector = new Conector(60 * 2); //2 minutos.
            HtmlNodeCollection[] htmlNodes = conector.IntenaVariosNodos(LinkPaginaPrincipal, Configuracion.xPathsTitulo, Configuracion.xPathsLinks);

            //Referencias:
            HtmlNodeCollection nodosTitulo = htmlNodes[0];
            HtmlNodeCollection nodosLinksCapitulos = htmlNodes[1];
            string Titulo = ObtenInnerText(nodosTitulo);
            List<string> LinksDeCapitulos = ObtenLinks(nodosLinksCapitulos, OrdenLinks.Descendiente);
            float numPrimerCap = EncuentraInformacionCapitulo(LinksDeCapitulos.First()).NumeroCapitulo;
            float numUltimoCap = EncuentraInformacionCapitulo(LinksDeCapitulos.Last()).NumeroCapitulo;

            //Ordeanando la información:
            InformacionNovela info = new InformacionNovela(Titulo, LinkPaginaPrincipal, LinksDeCapitulos, numPrimerCap, numUltimoCap);

            return info;
        }


        #region Private

        /// <summary>
        /// Ayuda a definir la manera que se enlistan los links.
        /// </summary>
        private enum OrdenLinks { Ascendente, Descendiente }


        /// <summary>
        /// Obtiene los links de una lista de nodos.
        /// </summary>
        /// <param name="nodosLinksCapitulos">Nodos de los que se extraen</param>
        /// <param name="orden">Orden que se enlistan</param>
        /// <returns></returns>
        private static List<string> ObtenLinks(HtmlNodeCollection nodosLinksCapitulos, OrdenLinks orden = OrdenLinks.Descendiente)
        {
            List<string> LinksDeCapitulos = new List<string>();

            if (orden == OrdenLinks.Descendiente)
            {
                for (int i = nodosLinksCapitulos.Count - 1; i > -1; i--)
                {
                    HtmlNode node = nodosLinksCapitulos[i];
                    LinksDeCapitulos.Add(node.Attributes["href"].Value);
                }
            }
            else
            {
                for (int i = 0; i < nodosLinksCapitulos.Count; i++)
                {
                    HtmlNode node = nodosLinksCapitulos[i];
                    LinksDeCapitulos.Add(node.Attributes["href"].Value);
                }
            }


            return LinksDeCapitulos;
        }


        /// <summary>
        /// Obtiene el inner text de un nodo y le quita las cosas no deseadas.
        /// </summary>
        /// <param name="nodosTitulo">Nodo a editar.</param>
        /// <returns></returns>
        private static string ObtenInnerText(HtmlNodeCollection nodosTitulo)
        {
            //Tomando información:
            string Titulo = HttpUtility.HtmlDecode(nodosTitulo.FirstOrDefault().InnerText);
            Titulo = Titulo.Replace("\n", "").Replace("\t", "").Trim();
            return Titulo;
        }

        #endregion

        //public static InformacionNovela EncuentraInformacionNovela(string LinkPaginaPrincipal)
        //{
        //    Conector conector = new Conector(60 * 2);
        //    HtmlNodeCollection[] htmlNodes = conector.IntenaVariosNodos(LinkPaginaPrincipal, Configuracion.xPathsTitulo, Configuracion.xPathsLinks);
        //    HtmlDocument website = conector.HardConnect(LinkPaginaPrincipal);

        //    //Titulo:
        //    HtmlNodeCollection nodosTitulo = conector.ObtenNodes(website, Configuracion.xPathsTitulo);
        //    string Titulo = HttpUtility.HtmlDecode(nodosTitulo.FirstOrDefault().InnerText);
        //    HtmlNodeCollection nodosLinksCapitulos = conector.ObtenNodes(website, Configuracion.xPathsLinks);

        //    //Tomando información:
        //    Titulo = Titulo.Replace("\n", "").Replace("\t", "").Trim();

        //    List<string> LinksDeCapitulos = new List<string>();
        //    for (int i = nodosLinksCapitulos.Count - 1; i > -1; i--)
        //    {
        //        HtmlNode node = nodosLinksCapitulos[i];
        //        LinksDeCapitulos.Add(node.Attributes["href"].Value);
        //    }

        //    float PrimerCapitulo = EncuentraInformacionCapitulo(LinksDeCapitulos.First()).NumeroCapitulo;
        //    float UltimoCapitulo = EncuentraInformacionCapitulo(LinksDeCapitulos.Last()).NumeroCapitulo;


        //    InformacionNovela infoNovela = new InformacionNovela(Titulo, LinkPaginaPrincipal, LinksDeCapitulos, PrimerCapitulo, UltimoCapitulo);

        //    return infoNovela;
        //}


    }
}
