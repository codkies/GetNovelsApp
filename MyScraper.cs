using System;
using System.Collections.Generic;
using System.Web;
using HtmlAgilityPack;

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

    public bool HayOtroCapitulo => PruebaLink(SiguienteCapitulo);

    private HtmlWeb Conexion;

    string xPath; 
    #endregion


    #region Core

    public void InicializaScrapper(string xPath)
    {
        Console.WriteLine($"Scraper--> Comenzando conexion ///");
        Conexion = new HtmlWeb();
        this.xPath = xPath;
    }

    private string ObtenCapitulo(string direccion, int indexCapitulo)
    {      
        Console.WriteLine($"Scraper--> Capitulo {indexCapitulo} comenzando. Direccion: {direccion} ///");

        bool exito = true;
        DireccionActual = direccion;        

        string capitulo = ScrappDireccion(direccion, ref exito);
        if (!capitulo.Equals(""))
        {   
            indexCapitulo++;
            CapitulosEncontrados++;            
            CaracteresVistos += capitulo.Length;
            Console.WriteLine($"Scraper--> Capitulo {indexCapitulo}, finalizado. Tiene {capitulo.Length} caracteres. ///");            
        }
        else
        {
            Console.WriteLine($"Scraper-->  Capitulo {indexCapitulo}, error. Deteniendo conexión. !!!");
        }       
        
        return capitulo;
    }

    private string ScrappDireccion(string direccion, ref bool exito)
    {
        HtmlDocument doc = Conexion.Load(direccion);

        List<string> CapituloDesordenado = new List<string>();
        foreach (var item in doc.DocumentNode.SelectNodes(xPath))
        {
            string entrada = item.InnerText;
            bool paso = RevisaEntrada(entrada);
            if (paso) CapituloDesordenado.Add(entrada);
        }

        exito = CapituloDesordenado.Count > 0;

        if (exito)
        {
            string capitulo = OrdenaCapitulo(CapituloDesordenado);
            return capitulo;
        }
        else
        {
            return "";
        }
    }

    #endregion

    #region Helpers

    private bool PruebaLink(string direccion)
    {
        Console.WriteLine($"Scraper--> Revisando si hay un siguiente capitulo. ///");
        HtmlDocument doc = Conexion.Load(direccion);
        
        bool Hay = doc.DocumentNode.SelectNodes(xPath).Count > 0;

        if (Hay)
        {
            Console.WriteLine($"Scraper--> Si existe un siguiente capitulo. ///");
        }
        else
        {
            Console.WriteLine($"Scraper--> No existe un siguiente capitulo. !!!");
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
                "Edited", "Translated", "Editor", "Translator"
            };

        foreach (string checks in Checks)
        {
            if (entrada.Contains(checks))
            {
                EntradasIgnoradas++;
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
        /*LINK: https://www.readlightnovel.org/versatile-mage/chapter-01 */

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

