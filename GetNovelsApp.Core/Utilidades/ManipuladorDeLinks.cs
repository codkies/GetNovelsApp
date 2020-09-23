using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="DireccionAProbar"></param>
        /// <returns></returns>
        public static InformacionCapitulo EncuentraInformacionCapitulo(string DireccionAProbar)
        {
            /*
             Logrado hacer que reconozca los puntos.
             */
            string CapituloEscrito = string.Empty;

            string TituloCapitulo = string.Empty;

            int gruposDeNumeros = 0;

            for (int i = 0; i < DireccionAProbar.Length; i++)
            {
                //Preparations.
                char letra = DireccionAProbar[i];
                bool EsUnNumero = char.IsDigit(letra);
                //CapituloEscrito = string.Empty;

                if (!EsUnNumero & gruposDeNumeros == 1 & letra.Equals('-'))
                {
                    letra = '.';
                }
                else if (!EsUnNumero) continue;

                gruposDeNumeros++;
                //Haciendo un check de que hayan mas caracteres
                if (i == DireccionAProbar.Length - 1)
                {
                    CapituloEscrito += letra.ToString();
                    TituloCapitulo += letra.ToString();
                    break;
                }
                //--------------------------------------------------------------

                //Core:
                CapituloEscrito += letra.ToString(); //1  

                //Revisando los siguientes caracteres.
                int salto = i + 1;
                bool subio = false;
                for (int x = salto; x < DireccionAProbar.Length; x++)
                {
                    char letraFutura = DireccionAProbar[x];
                    if (char.IsDigit(letraFutura)) //Solo procede si el caracter es un #
                    {
                        CapituloEscrito += letraFutura.ToString();//2     
                        salto = x;
                        subio = true;
                    }
                    else break;//Apenas halles una letra, rompe este loop.
                }
                TituloCapitulo = CapituloEscrito;
                i = subio ? salto : i; //Si el valor subió, has que la siguiente iteracion continue ahí. Sino, deja que continue normalmente.
            }

            //Posibles errores:
            if (gruposDeNumeros < 1) Mensajero.MuestraErrorMayor("No se pudo determinar el valor del capitulo.");
            //---------------------


            float NumeroCapitulo = Math.Abs(float.Parse(CapituloEscrito));
            int Valor = gruposDeNumeros;
            TituloCapitulo = $"Chapter {TituloCapitulo}";

            InformacionCapitulo infoDelCapituloSegunElLink = new InformacionCapitulo(TituloCapitulo, Valor, NumeroCapitulo);

            return infoDelCapituloSegunElLink;
        }


        public static InformacionNovela EncuentraInformacionNovela(string LinkPaginaPrincipal)
        {
            Conector conector = new Conector(60 * 2);

            HtmlDocument website = conector.HardConnect(LinkPaginaPrincipal);

            //Titulo:
            HtmlNodeCollection nodosTitulo = conector.ObtenNodes(website, Configuracion.xPathsTitulo);
            string Titulo = HttpUtility.HtmlDecode(nodosTitulo.FirstOrDefault().InnerText);
            HtmlNodeCollection nodosLinksCapitulos = conector.ObtenNodes(website, Configuracion.xPathsLinks);

            //Tomando información:
            Titulo = Titulo.Replace("\n", "").Replace("\t", "").Trim();

            List<string> LinksDeCapitulos = new List<string>();
            for (int i = nodosLinksCapitulos.Count - 1; i > -1; i--)
            {
                HtmlNode node = nodosLinksCapitulos[i];
                LinksDeCapitulos.Add(node.Attributes["href"].Value);
            }

            float PrimerCapitulo = EncuentraInformacionCapitulo(LinksDeCapitulos.First()).NumeroCapitulo;
            float UltimoCapitulo = EncuentraInformacionCapitulo(LinksDeCapitulos.Last()).NumeroCapitulo;


            InformacionNovela infoNovela = new InformacionNovela(Titulo, LinkPaginaPrincipal, LinksDeCapitulos, PrimerCapitulo, UltimoCapitulo);

            return infoNovela;
        }

    }
}
