using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;
using GetNovelsApp.Modelos;
using GetNovelsApp.Utilidades;
using System.IO;
using System;

namespace GetNovelsApp
{
    public class Scraper
    {
        #region Props & Fields

        //Informacion
        public int EntradasIgnoradas { get; private set; } = 0;

        public long CaracteresVistos { get; private set; } = 0;

        public int CapitulosEncontrados { get; private set; } = 0;

        public string SiguienteDireccion => EncuentraSiguienteCapitulo();

        List<string> xPaths => Configuracion.xPaths;


        #endregion

        #region Fields

        Configuracion Configuracion;

        int IndexCapitulos;

        private string DireccionActual;

        private HtmlWeb Conexion;

        #endregion


        #region Public

        public void InicializaScrapper(Configuracion configuracion)
        {
            Configuracion = configuracion;
            Mensajero.MuestraNotificacion($"Scraper--> Comenzando conexion.");
            Conexion = new HtmlWeb();
            IndexCapitulos = 1;
        }

        public Capitulo ObtenCapitulo(string direccion, int indexCapitulo)
        {
            Mensajero.MuestraNotificacion($"Scraper--> Capitulo {indexCapitulo} comenzando. Direccion: {direccion}.");

            bool exito = true;
            DireccionActual = direccion;
            IndexCapitulos = indexCapitulo;

            Capitulo capitulo = ScrappDireccion(DireccionActual, ref exito);
            if (!capitulo.Equals(""))
            {
                CapitulosEncontrados++;
                CaracteresVistos += capitulo.Caracteres;
                Mensajero.MuestraExito($"Scraper--> Capitulo {IndexCapitulos}, finalizado. Tiene {capitulo.Caracteres} caracteres.");
            }
            else
            {
                Mensajero.MuestraError($"Scraper-->  Capitulo {indexCapitulo}, error. Deteniendo conexión.");
            }

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
                Mensajero.MuestraError("No se encontraron nodes con los xPaths ingresados. Presiona enter para cerrar el programa.");
                Console.ReadLine();
                Environment.Exit(0);
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
                Capitulo capitulo = new Capitulo(texto, IndexCapitulos);
                return capitulo;
            }
            else
            {
                return new Capitulo(string.Empty, -1);
            }
        }

        private string EncuentraSiguienteCapitulo()
        {
            string posibleSiguienteDireccion = SumaUnoALink(DireccionActual);
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
                Mensajero.MuestraError($"Scraper--> No existe un siguiente capitulo. Probando agregando sufijo -end");
                posibleSiguienteDireccion += "-end";

                doc = Conexion.Load(posibleSiguienteDireccion);
                posibleColeccion = null;
                foreach (string xPath in xPaths)
                {
                    posibleColeccion = doc.DocumentNode.SelectNodes(xPath);
                    if (posibleColeccion != null) break;
                }

                Hay = posibleColeccion?.Count > 0;
            }

            if (!Hay) posibleSiguienteDireccion = string.Empty;

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
                    if (i == direccionNueva.Length - 1) break; //Si es el ultimo i, rompe el loop.

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

                    //Revisa la cantidad de ceros a la izquierda.
                    /*ie: 
                    original = 007
                    longitudMinima = 3

                    capituloEncontrado = 8.
                    longitudNueva = 1

                    ceros= 2;
                    final = 008.
                    */

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
                    Mensajero.MuestraError($"Scraper--> {EntradasIgnoradas} entradas ignoradas.");
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


        public string xPathToNextLink { get; private set; } = "//a[@class = 'next next-link' ]";

        private string test_EncuentraSiguienteCap(string direccionActual)
        {
            HtmlDocument doc = Conexion.Load(direccionActual);

            string link = string.Empty;

            foreach (var item in doc.DocumentNode.SelectNodes(xPathToNextLink))
            {
                //Cargar esto cuando se hace scrapea.
                //link = item.GetAttributeValue("href", string.Empty);
                link = item.Attributes["href"].Value;
                if (link.Equals(string.Empty)) continue;
                else break;
            }

            if (link.Equals(string.Empty)) Mensajero.MuestraError("Siguiente link no encontrado");

            return link;
        }


        #endregion

    }
}