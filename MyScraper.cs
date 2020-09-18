using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using HtmlAgilityPack;
using iText.Kernel.XMP.Impl.XPath;

public struct Capitulo
{
    public string Texto;
    public int NumeroCapitulo;

    public int Caracteres => Texto.Length;

    public Capitulo(string texto, int numeroCapitulo)
    {
        Texto = texto;
        NumeroCapitulo = numeroCapitulo;
    }
}

public class MyScraper
{
    #region Props & Fields

    //Informacion
    public int EntradasIgnoradas { get; private set; } = 0;

    public Int64 CaracteresVistos { get; private set; } = 0;

    public int CapitulosEncontrados { get; private set; } = 0;


    //Configuracion
    private string DireccionActual;

    public string SiguienteCapitulo => EncuentraSiguienteCap(DireccionActual);

    public bool HayOtroCapitulo => PruebaLink();

    private HtmlWeb Conexion;

    List<string> xPaths = new List<string>();

    int IndexCapitulos;

    #endregion


    #region Public

    public void InicializaScrapper(List<string> xPaths)
    {
        Mensajero.MuestraNotificacion($"Scraper--> Comenzando conexion.");
        Conexion = new HtmlWeb();
        this.xPaths = xPaths;
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

    private bool PruebaLink()
    {
        HtmlDocument doc = Conexion.Load(DireccionActual);

        HtmlNodeCollection posibleColeccion = null;
        foreach (string xPath in xPaths)
        {
            posibleColeccion = doc.DocumentNode.SelectNodes(xPath);
            if (posibleColeccion != null) break;
        }

        bool Hay = posibleColeccion.Count > 0;

        if (!Hay)
        {
            Mensajero.MuestraError($"Scraper--> No existe un siguiente capitulo. Probando agregando sufijo -end");
            DireccionActual += "-end";

            posibleColeccion = null;
            foreach (string xPath in xPaths)
            {
                posibleColeccion = doc.DocumentNode.SelectNodes(xPath);
                if (posibleColeccion != null) break;
            }

            Hay = posibleColeccion.Count > 0;
        } 
        return Hay;
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


    private string EncuentraSiguienteCap(string direccionCapAnterior)
    {
        //Regresa "" si no encuentras nada.

        string direccionNueva = string.Empty;
        string capitulo = string.Empty;

        for (int i = 0; i < direccionCapAnterior.Length; i++)
        {
            char letra = direccionCapAnterior[i];
            string letra_ = letra.ToString();
            bool EsUnNumero = char.IsDigit(letra);
            if (EsUnNumero)
            {
                /*Encontrando el primero numero y revisando si es 0*/
                capitulo += letra.ToString(); //1  

                //Haciendo un check de que hayan mas caracteres
                if (i == direccionNueva.Length - 1) break; //Si es el ultimo i, rompe el loop.

                //Revisando los siguientes caracteres.
                int siguiente = i + 1;
                for (int x = siguiente; x < direccionCapAnterior.Length; x++)
                {
                    char letraFutura = direccionCapAnterior[x];
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
                i = siguiente + 1 < direccionCapAnterior.Length - 1 ? siguiente + 1 : direccionCapAnterior.Length;
            }
            else
            {
                direccionNueva += letra.ToString();
            }

        }

        return direccionNueva;
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

        if (link.Equals(string.Empty)) Console.WriteLine("Siguiente link no encontrado");
        
        return link;
    } 


    #endregion




    #region Legacy
    /// <summary>
    /// Esta version no soporta si los capitulos los escriben con un 0 a la izquierda
    /// </summary>
    /// <param name="direccionCapAnterior"></param>
    /// <returns></returns>
    private string legacy_EncuentraSiguienteCap(string direccionCapAnterior)
    {
        //Regresa "" si no encuentras nada.
        /*LINK: https://www.readlightnovel.org/versatile-mage/chapter-1   */

        string direccionNueva = string.Empty;
        string capitulo = string.Empty;

        for (int i = 0; i < direccionCapAnterior.Length; i++)
        {
            char letra = direccionCapAnterior[i];
            string letra_ = letra.ToString();
            bool EsUnNumero = char.IsDigit(letra);
            if (EsUnNumero)
            {
                capitulo += letra.ToString(); //1

                if (i == direccionNueva.Length - 1) break; //Si es el ultimo i, rompe el loop.

                int end = i + 1;
                for (int x = end; x < direccionCapAnterior.Length; x++)
                {
                    char letraFutura = direccionCapAnterior[x];
                    if (char.IsDigit(letraFutura))
                    {
                        capitulo += letraFutura.ToString();//2
                        end = x;
                    }
                    else break;//Apenas halles una letra, rompe este loop.
                }
                capitulo = (int.Parse(capitulo) + 1).ToString(); //Conviertelo a INT, sumale 1 y metelo de nuevo en el link.
                direccionNueva += capitulo;
                i = end + 1 < direccionCapAnterior.Length - 1 ? end + 1 : direccionCapAnterior.Length;
            }
            else
            {
                direccionNueva += letra.ToString();
            }

        }

        return direccionNueva;
    } 

    #endregion
}

