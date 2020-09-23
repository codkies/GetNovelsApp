using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;
using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Conexiones;
using System.Linq;
using System;

namespace GetNovelsApp.Core
{
    public class Scraper
    {
        #region Constructor

        public Scraper()
        {
            Mensajero.MuestraNotificacion($"Scraper --> Creando conector.");
            conector = new Conector(tiempoTopeEnSegundos: 300); //5 minutos de tiempo de espera
        }

        #endregion


        #region Props

        //Informacion
        public int EntradasIgnoradas { get; private set; } = 0;

        public long CaracteresVistos { get; private set; } = 0;

        public int CapitulosEncontrados { get; private set; } = 0;


        #endregion


        #region Fields


        private string DireccionActual;


        /// <summary>
        /// Instancia inicializada en el constructor del Scraper.
        /// </summary>
        private Conector conector;


        private HtmlDocument DocActual;


        private HtmlDocument DocSiguiente;

        #endregion


        #region Capitulo Actual

        public Capitulo ObtenCapitulo(string direccion)
        {
            DireccionActual = direccion;

            List<string> textosRaw = ObtenTextoRaw(DireccionActual);
            string Texto = OrdenaTextoRaw(textosRaw);
            Capitulo capitulo = new Capitulo(Texto, direccion);

            CapitulosEncontrados++;
            CaracteresVistos += capitulo.Caracteres;
            //Mensajero.MuestraNotificacion($"Scraper --> {capitulo.TituloCapitulo} tiene {capitulo.Caracteres} caracteres.");

            return capitulo;
        }


        private List<string> ObtenTextoRaw(string direccion)
        {
            HtmlNodeCollection nodos = conector.IntentaNodos(direccion, Configuracion.xPathsTextos);       

            List<string> CapituloDesordenado = ObtenInnerText(nodos);

            if (CapituloDesordenado.Any()) return CapituloDesordenado;
            else return null;
        }


        private List<string> ObtenInnerText(HtmlNodeCollection nodes)
        {
            List<string> CapitulosDesordenado = new List<string>();

            foreach (var item in nodes)
            {
                string entrada = item.InnerText;
                bool paso = RevisaEntrada(entrada);
                if (paso) CapitulosDesordenado.Add(entrada);
            }

            return CapitulosDesordenado;
        }


        private string OrdenaTextoRaw(List<string> capituloDesordenado)
        {
            string capituloOrdenado = string.Empty;
            foreach (string entrada in capituloDesordenado)
            {
                var x = HttpUtility.HtmlDecode(entrada);
                capituloOrdenado += $"{x}\n\n";
            }
            return capituloOrdenado;
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

        #endregion




        #region Legacy

        /*
        private string SiguienteDireccion => EncuentraSiguienteCapitulo();

        private string EncuentraSiguienteCapitulo()
        {
            Mensajero.MuestraNotificacion("Scraper --> Ejecutando busqueda de siguiente episodio en botones del actual");
            //Probando 2 maneras de conseguir el siguiente link
            string posibleSiguienteDireccion = BuscaBotonNext(DocActual);

            if (posibleSiguienteDireccion.Equals(string.Empty))
            {
                Mensajero.MuestraCambioEstado("Scraper --> No se encontró un boton de siguiente episodio. Se intentará adivinar el siguiente link...");
                posibleSiguienteDireccion = SumaUnoALink(DireccionActual);
            }

            bool encontrada = PruebaDireccion(posibleSiguienteDireccion, ref DocSiguiente, out HtmlNodeCollection _);

            if (!encontrada)//Si estas 2 maneras no funcionaron, se prueba una tercera:
            {
                Mensajero.MuestraError($"Scraper --> No se pudo adivinar el siguiente link... Probando agregando sufijo -end");

                posibleSiguienteDireccion = SumaEnd(posibleSiguienteDireccion);
                bool encontrada2 = PruebaDireccion(posibleSiguienteDireccion, ref DocSiguiente, out HtmlNodeCollection _);

                if (!encontrada2)
                {
                    Mensajero.MuestraError($"Scraper --> No se encontró un siguiente capitulo.");
                    posibleSiguienteDireccion = string.Empty;
                }
                else
                {
                    Mensajero.MuestraCambioEstado($"Scraper --> Se encontró un siguiente capitulo agregando el sufijo -end.");
                }
            }
            else Mensajero.MuestraCambioEstado($"Scraper --> Se encontró la dirección.");

            return posibleSiguienteDireccion;
        }


        /// <summary>
        /// Prueba si una dirección se le encuentra algo.
        /// </summary>
        /// <param name="posibleSiguienteDireccion"></param>
        /// <param name="DocSiguiente"></param>
        /// <param name="posibleColeccion"></param>
        /// <returns></returns>
        private bool PruebaDireccion(string posibleSiguienteDireccion, ref HtmlDocument DocSiguiente, out HtmlNodeCollection posibleColeccion)
        {
            Mensajero.MuestraCambioEstado("Scraper --> Comprobando siguiente dirección.");
            DocSiguiente = conector.HardConnect(posibleSiguienteDireccion);
            conector.ObtenNodes(DocSiguiente, Configuracion.xPathsTextos, out HtmlNodeCollection _posibleColeccion);
            
            throw new NotImplementedException();
        }


        private string SumaEnd(string posibleSiguienteDireccion)
        {
            posibleSiguienteDireccion += "-end";
            return posibleSiguienteDireccion;
        }


        /// <summary>
        /// Busca en el DocActual los xPathsToNextLink
        /// </summary>
        /// <param name="DocActual">HtmlDocument de la pagina en donde buscarlos</param>
        /// <returns></returns>
        private string BuscaBotonNext(HtmlDocument DocActual)
        {
            HtmlNodeCollection posiblesNodos = conector.ObtenNodes(DocActual, Configuracion.xPathsLinks);            

            string link = string.Empty;

            foreach (HtmlNode nodo in posiblesNodos)
            {
                string posibleLink = nodo.Attributes["href"].Value;
                if (!posibleLink.Equals(string.Empty))
                {
                    link = posibleLink;
                    break;
                }
            }
            return link;
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
                    //Encontrando el primero numero y revisando si es 0
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
        */
        #endregion
    }
}