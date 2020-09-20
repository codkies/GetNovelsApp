using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;
using GetNovelsApp.Core.Utilidades;
using System.IO;
using System;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core
{
    public class Scraper
    {
        #region Constructor

        public Scraper(Configuracion configuracion)
        {
            Configuracion = configuracion;
            Mensajero.MuestraNotificacion($"Scraper --> Comenzando conexion.");
            Conexion = new HtmlWeb();
        }

        #endregion

        #region Props & Fields

        //Informacion
        public int EntradasIgnoradas { get; private set; } = 0;

        public long CaracteresVistos { get; private set; } = 0;

        public int CapitulosEncontrados { get; private set; } = 0;

        public string SiguienteDireccion => EncuentraSiguienteCapitulo();

        private List<string> xPaths => Configuracion.xPaths;


        #endregion

        #region Fields

        Configuracion Configuracion;


        private string DireccionActual;

        private HtmlWeb Conexion;

        #endregion


        #region Public
        public Capitulo ObtenCapitulo(string direccion)
        {
            bool exito = true;
            DireccionActual = direccion;

            Capitulo capitulo = ScrappDireccion(DireccionActual, ref exito);

            if (capitulo == null)
            {
                Mensajero.MuestraErrorMayor($"Scraper -->  Direccion {DireccionActual}, error. No se encontraron items con los xPaths establecidos.");
            }

            CapitulosEncontrados++;
            CaracteresVistos += capitulo.Caracteres;
            Mensajero.MuestraNotificacion($"Scraper --> Capitulo número {capitulo.NumeroCapitulo} tiene {capitulo.Caracteres} caracteres.");

            return capitulo;
        }

        #endregion

        #region Private

        private Capitulo ScrappDireccion(string direccion, ref bool exito)
        {
            HtmlDocument doc = Conexion.Load(direccion);

            List<string> CapituloDesordenado = new List<string>();

            HtmlNodeCollection posibleColeccion = null;
            foreach (string xPath in xPaths)
            {
                posibleColeccion = doc.DocumentNode.SelectNodes(xPath);
                if (posibleColeccion != null) break;
            }

            if(posibleColeccion == null)
            {
                Mensajero.MuestraErrorMayor("Scraper --> No se encontraron nodes con los xPaths ingresados. Presiona enter para cerrar el programa.");
            }

            foreach (var item in posibleColeccion)
            {
                string entrada = item.InnerText;
                bool paso = RevisaEntrada(entrada);
                if (paso) CapituloDesordenado.Add(entrada);
            }

            exito = CapituloDesordenado.Count > 0;

            if (exito)
            {
                string texto = OrdenaCapitulo(CapituloDesordenado);
                Capitulo capitulo = new Capitulo(texto, direccion);
                return capitulo;
            }
            else
            {
                return null;
            }
        }

        private string EncuentraSiguienteCapitulo()
        {
            string posibleSiguienteDireccion = BuscaBotonNext(DireccionActual);
            if (posibleSiguienteDireccion.Equals(string.Empty))
            {
                Mensajero.MuestraCambioEstado("Scraper --> No se encontró un boton de siguiente episodio. Se intentará adivinar el siguiente link...");
                posibleSiguienteDireccion = SumaUnoALink(DireccionActual);
            }

            HtmlDocument doc = Conexion.Load(posibleSiguienteDireccion);

            HtmlNodeCollection posibleColeccion = null;
            foreach (string xPath in xPaths)
            {
                posibleColeccion = doc.DocumentNode.SelectNodes(xPath);
                if (posibleColeccion != null) break;
            }
            
            bool Hay = posibleColeccion?.Count > 0;

            if (!Hay)
            {
                Mensajero.MuestraError($"Scraper --> No se encontró un siguiente capitulo... Probando agregando sufijo -end");
                posibleSiguienteDireccion += "-end";

                doc = Conexion.Load(posibleSiguienteDireccion);
                posibleColeccion = null;
                foreach (string xPath in xPaths)
                {
                    posibleColeccion = doc.DocumentNode.SelectNodes(xPath);
                    if (posibleColeccion != null) break;
                }

                if (posibleColeccion?.Count < 1)
                {
                    Mensajero.MuestraError($"Scraper --> No se encontró un siguiente capitulo.");
                    posibleSiguienteDireccion = string.Empty;
                }
            }  
            else Mensajero.MuestraCambioEstado($"Scraper --> Se encontró la dirección.");

            return posibleSiguienteDireccion;
        }


        private string SumaUnoALink(string DireccionAProbar)
        {   
            string direccionNueva = string.Empty;
            string capitulo = string.Empty;

            for (int i = 0; i < DireccionAProbar.Length; i++)
            {
                char letra = DireccionAProbar[i];
                bool EsUnNumero = char.IsDigit(letra);
                if (EsUnNumero)
                {
                    /*Encontrando el primero numero y revisando si es 0*/
                    capitulo += letra.ToString(); //1  

                    //Haciendo un check de que hayan mas caracteres
                    if (i == DireccionAProbar.Length - 1) break; //Si es el ultimo i, rompe el loop.

                    //Revisando los siguientes caracteres.
                    int siguiente = i + 1;
                    for (int x = siguiente; x < DireccionAProbar.Length; x++)
                    {
                        char letraFutura = DireccionAProbar[x];
                        if (char.IsDigit(letraFutura)) //Solo procede si el caracter es un #
                        {
                            capitulo += letraFutura.ToString();//2                            
                            siguiente = x;
                        }
                        else break;//Apenas halles una letra, rompe este loop.
                    }

                    //Toma la longitud de los caracteres originales
                    int longitudMinima = capitulo.Length;

                    //Toma el digito del capitulo actual, sumale uno, y devuelvelo a un string.
                    capitulo = (int.Parse(capitulo) + 1).ToString(); //Conviertelo a INT, sumale 1 y metelo de nuevo en el link.

                    int longitudNueva = capitulo.Length;

                    int cantidadDeCeros = longitudMinima - longitudNueva;

                    if (cantidadDeCeros > 0)
                    {
                        for (int j = 0; j < cantidadDeCeros; j++)
                        {
                            capitulo = $"0{capitulo}";
                        }
                    }                  

                    direccionNueva += capitulo;
                    i = siguiente + 1 < DireccionAProbar.Length - 1 ? siguiente + 1 : DireccionAProbar.Length;
                }
                else
                {
                    direccionNueva += letra.ToString();
                }

            }

            return direccionNueva;
        }


        /// <summary>
        /// Revisa si una entrada pasa los Checks
        /// </summary>
        /// <param name="entrada"></param>
        /// <returns></returns>
        private bool RevisaEntrada(string entrada)
        {
            List<string> Checks = new List<string>()
            {
                "Edited by", "Translated by", "Editor:", "Translator:"
            };

            foreach (string checks in Checks)
            {
                if (entrada.Contains(checks))
                {
                    EntradasIgnoradas++;
                    Mensajero.MuestraError($"Scraper --> {EntradasIgnoradas} entradas ignoradas.");
                    return false;
                }
            }
            return true;
        }


        private string OrdenaCapitulo(List<string> capituloDesordenado)
        {
            string capituloOrdenado = string.Empty;
            foreach (string entrada in capituloDesordenado)
            {
                var x = HttpUtility.HtmlDecode(entrada);
                capituloOrdenado += $"{x}\n\n";
            }
            return capituloOrdenado;
        }



        #endregion


        #region New       

        private List<string> xPathsToNextLink => Configuracion.xPathsBotonSiguiente;

        /// <summary>
        /// Busca en la direccion ingresada, el boton para pasar de pagina. Regresa el link de ese boton. If not, regresa string.empty.
        /// </summary>
        /// <param name="direccionActual"></param>
        /// <returns></returns>
        private string BuscaBotonNext(string direccionActual)
        {
            HtmlDocument doc = Conexion.Load(direccionActual);

            string link = string.Empty;
            HtmlNodeCollection posiblesNodos = null;

            foreach (string xPath in xPathsToNextLink)
            {
                posiblesNodos = doc.DocumentNode.SelectNodes(xPath);

                if (posiblesNodos == null) continue;
                if (posiblesNodos.Count < 1) continue;

                string posibleLink = posiblesNodos[0].Attributes["href"].Value;

                if (!posibleLink.Equals(string.Empty))
                {
                    link = posibleLink;
                    break;
                }             
            }

            return link;
        }


        #endregion

    }
}