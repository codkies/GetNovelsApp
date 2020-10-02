using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using GetNovelsApp.Core.Reportaje;
using HtmlAgilityPack;
using iText.Kernel.Pdf.Tagutils;

namespace GetNovelsApp.Core.Conexiones.Internet
{

    public static class ManipuladorDeLinks
    {
        #region Cosas de reporter
        /// <summary>
        /// Cree esta clase solo para tener un reportero para pasar.
        /// </summary>
        private class LinkScraper : IReportero
        {
            public string Nombre => "LinkScraper";
        }

        private readonly static LinkScraper MiReportero = new LinkScraper();

        #endregion


        /// <summary>
        /// Encuentra el numero del capitulo según el link.
        /// </summary>
        /// <param name="LinkCapitulo"></param>
        /// <returns></returns>
        public static CapituloWebModel EncuentraInformacionCapitulo(Uri _LinkCapitulo)
        {
            string numCapEscrito = string.Empty; //Para llevar record de numeros en formato de string. Luego se convierte a float.
            string TituloCapitulo = string.Empty; //Titulo del cap.
            int gruposDeNumeros = 0; //Grupos de numeros en el Link. ie: website.com/10-12/ ->> tiene 2 grupos de numeros.
            string LinkCapitulo = _LinkCapitulo.ToString();

            for (int i = 0; i < LinkCapitulo.Length; i++)
            {
                //Preparations.
                char letra = LinkCapitulo[i];
                bool EsUnNumero = char.IsDigit(letra);

                if (gruposDeNumeros == 1 & (letra.Equals('-') | letra.Equals('_')) )
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
                //Esto romperá la app si varios mensajes (async) ocurren al mismo tiempo?...
                GetNovelsComunicador.ReportaError("No se pudo determinar el valor del capitulo.", MiReportero);
                GetNovelsComunicador.Reporta($"La dirección es: \n{LinkCapitulo}.", MiReportero);

                string inputUserTitulo = string.Empty;
                string inputUserNumCap = string.Empty;
                string inputUserValorCap = string.Empty;

                bool decisionTomada = false;
                while (!decisionTomada)
                {
                    inputUserTitulo = GetNovelsComunicador.PideInput($"\nEscribe el titulo del capitulo: (En general es 'Chapter - (numeroCapitulo)')", MiReportero);
                    inputUserNumCap = GetNovelsComunicador.PideInput($"Escribe el numero del capitulo: (puede tener decimales pero no acepta letras):", MiReportero);
                    inputUserValorCap = GetNovelsComunicador.PideInput($"Escribe por cuantos capitulos vale este: (si es un solo cap, el valor es 1. No acepta decimales ni letras):", MiReportero);

                    GetNovelsComunicador.Reporta("Has escrito:\n" +
                                                    $"Direccion: {LinkCapitulo}\n" +
                                                    $"Titulo cap: {inputUserTitulo}\n" +
                                                    $"Numero del capitulo: {inputUserNumCap}\n" +
                                                    $"Valor del capitulo: {inputUserValorCap}",
                                                    MiReportero);
                    string decision = GetNovelsComunicador.PideInput("Presiona (Y) para confirmar. Cualquier otra tecla para repetir.", MiReportero);

                    if (decision.Equals("y") | decision.Equals("yes"))
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

            CapituloWebModel infoDelCapituloSegunElLink = new CapituloWebModel(_LinkCapitulo, TituloCapitulo, Valor, NumeroCapitulo);

            return infoDelCapituloSegunElLink;
        }


        public static InformacionNovelaOnline EncuentraInformacionNovela(Uri LinkPaginaPrincipal)
        {
            //Conexiones:
            Conector conector = new Conector(60 * 2); //2 minutos.
            HtmlNodeCollection[] htmlNodes = conector.IntenaVariosNodos(LinkPaginaPrincipal, GetNovelsConfig.xPathsTitulo, GetNovelsConfig.xPathsLinks);

            //Titulo:
            HtmlNodeCollection nodosTitulo = htmlNodes[0];
            string Titulo = ObtenInnerText(nodosTitulo);

            //Buscando info en NovelUpdates
            BuscaNovelaEnNovelUpdates(Titulo, conector, out string Sipnosis, out List<string> Tags, out Uri LinkImagen);

            //Links:
            HtmlNodeCollection nodosLinksCapitulos = htmlNodes[1];
            List<Uri> LinksDeCapitulos = ObtenLinks(nodosLinksCapitulos, OrdenLinks.Descendiente);


            //Ordeanando la información:
            InformacionNovelaOnline info = new InformacionNovelaOnline(Titulo, LinkPaginaPrincipal, LinksDeCapitulos, LinkImagen, Sipnosis, Tags);

            return info;
        }



        #region Private

        /// <summary>
        /// Busca el titulo de una novela en NU y obtiene la sipnosis, tags e imagen.
        /// </summary>
        /// <param name="Titulo"></param>
        /// <param name="conector"></param>
        /// <param name="Sipnosis"></param>
        /// <param name="Tags"></param>
        /// <param name="LinkImagen"></param>
        private static void BuscaNovelaEnNovelUpdates(string Titulo, Conector conector, out string Sipnosis, out List<string> Tags, out Uri LinkImagen)
        {
            //Conexion
            Uri DirNovelUpdates = ObtenNovelUpdatesWebpage(Titulo);

            List<List<string>> xPaths = new List<List<string>>()
            {
                GetNovelsConfig.xPathsSipnosis,
                GetNovelsConfig.xPathsImagen,
                GetNovelsConfig.xPathsTags
            };
            List<HtmlNodeCollection> infoEnNU = conector.IntenaVariosNodos(DirNovelUpdates, xPaths);

            //Ordenando resultados
            HtmlNodeCollection nodosSipnosis = infoEnNU[0];
            HtmlNode nodoImagen = infoEnNU[1].First();
            HtmlNodeCollection nodosTags = infoEnNU[2];

            //Fitlrando sipnosis
            Sipnosis = string.Empty;
            foreach (HtmlNode nodo in nodosSipnosis)
            {
                Sipnosis += ObtenInnerText(nodo);
            }

            //Filtrando imagen
            LinkImagen = new Uri(nodoImagen.Attributes["src"].Value);

            //Tags
            Tags = new List<string>();
            foreach (HtmlNode node in nodosTags)
            {
                string _ = node.InnerText;
                Tags.Add(_);
            }

        }


        /// <summary>
        /// Obtiene la direccion de una novela en NovelUpdates
        /// </summary>
        /// <param name="Titulo"></param>
        /// <returns></returns>
        private static Uri ObtenNovelUpdatesWebpage(string Titulo)
        {
            var tituloSeparado = Titulo.Split();
            string direccionEnNU = "https://www.novelupdates.com/series/";

            for (int i = 0; i < tituloSeparado.Length; i++)
            {
                string p = tituloSeparado[i];
                if (i == tituloSeparado.Length - 1)
                {
                    direccionEnNU += $"{p}/";
                }
                else
                {
                    direccionEnNU += $"{p}-";
                }
            }

            return new Uri(direccionEnNU);
        }



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
        private static List<Uri> ObtenLinks(HtmlNodeCollection nodosLinksCapitulos, OrdenLinks orden)
        {
            List<Uri> LinksDeCapitulos = new List<Uri>();

            if (orden == OrdenLinks.Descendiente)
            {
                for (int i = nodosLinksCapitulos.Count - 1; i > -1; i--)
                {
                    HtmlNode node = nodosLinksCapitulos[i];
                    LinksDeCapitulos.Add(new Uri(node.Attributes["href"].Value));
                }
            }
            else
            {
                for (int i = 0; i < nodosLinksCapitulos.Count; i++)
                {
                    HtmlNode node = nodosLinksCapitulos[i];
                    LinksDeCapitulos.Add(new Uri(node.Attributes["href"].Value));
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
            foreach (HtmlNode node in nodosTitulo.First().SelectNodes("span"))
            {
                node.Remove();
            }
            string Titulo = HttpUtility.HtmlDecode(nodosTitulo.FirstOrDefault().InnerText);

            Titulo = Titulo.Replace("\n", "").Replace("\t", "").Trim();

            return Titulo;
        }


        private static string ObtenInnerText(HtmlNode nodo)
        {
            //Tomando información:
            string Titulo = HttpUtility.HtmlDecode(nodo.InnerText);
            Titulo = $" {Titulo.Replace("\n", "").Replace("\t", "").Trim()}"; //Agg un espacio
            return Titulo;
        }

        #endregion

    }
}
