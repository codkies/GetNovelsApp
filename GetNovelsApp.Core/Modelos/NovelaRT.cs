using System;
using System.Collections.Generic;
using System.Web.WebSockets;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Utilidades;
using iText.Layout.Borders;
using iText.StyledXmlParser.Jsoup.Select;
using Org.BouncyCastle.Asn1.Mozilla;

namespace GetNovelsApp.Core.Modelos
{
    /// <summary>
    /// Modelo que utiliza la app para conseguir capitulos y ordenarlos.
    /// </summary>
    public class NovelaRT : INovela
    {
        #region Setup

        public NovelaRT(List<Capitulo> capitulos, InformacionNovelaDB dbInfo)
        {
            OrganizaCapitulos(capitulos);

            ID = dbInfo.ID;
            Titulo = dbInfo.Titulo;
            LinkPrincipal = new Uri(dbInfo.LinkPrincipal);
            Tags = ManipuladorStrings.TagsEnLista(dbInfo.Tags);
            Sipnosis = dbInfo.Sipnosis;
            ImagenLink = new Uri(dbInfo.Imagen);
        }


        private void OrganizaCapitulos(List<Capitulo> capitulos)
        {
            foreach (Capitulo c in capitulos)
            {
                LinksDeCapitulos.Add(c.Link);

                if (string.IsNullOrEmpty(c.Texto) == false)
                {
                    CapitulosDescargados.Add(c);
                }
                else
                {
                    CapitulosPorDescargar.Add(c);
                }
            }
            OrdenaListas();
        }


        private void OrdenaListas()
        {
            CapitulosDescargados.Sort(new ComparerOrdenadorCapitulos());
            CapitulosPorDescargar.Sort(new ComparerOrdenadorCapitulos());
        }


        #endregion



        #region Fields

        public List<string> Tags { get; private set; } = new List<string>();
        public string Sipnosis { get; private set; }

        public Uri ImagenLink { get; private set; }

        /// <summary>
        /// ID en DB.
        /// </summary> 
        public int ID { get; private set; }

        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo { get; private set; }

        /// <summary>
        /// Link a su pagina principal de la novela
        /// </summary>
        public Uri LinkPrincipal { get; private set; }

        /// <summary>
        /// Lista de todos los links de los capitulos
        /// </summary>
        public List<Uri> LinksDeCapitulos { get; private set; } = new List<Uri>();

        /// <summary>
        /// Capitulos presentes en esta novela que no han sido metidos en el PDF
        /// </summary>
        public List<Capitulo> CapitulosDescargados { get; private set; } = new List<Capitulo>();


        /// <summary>
        /// Capitulos presentes en esta novela que ya han sido metidos en el PDF
        /// </summary>
        public List<Capitulo> CapitulosImpresos { get; private set; } = new List<Capitulo>();


        public List<Capitulo> CapitulosPorDescargar { get; private set; } = new List<Capitulo>();

        #endregion


        #region Propiedaes 
        /// <summary>
        /// Define si esta novela tiene capitulos por imprimir
        /// </summary>
        public bool TengoCapitulosPorImprimir => CapitulosDescargados.Count > 0;

        /// <summary>
        /// Define la cantidad de capitulos que esta novela tiene que no se han impreso.
        /// </summary>
        public int CantidadCapitulosDescargados => CapitulosDescargados.Count;


        /// <summary>
        /// Cantidad total de links que contiene esta novela.
        /// </summary>
        public int CantidadLinks => LinksDeCapitulos.Count;


        /// <summary>
        /// Todos los caps que esta novela tiene ref. (Caps, no links).
        /// </summary>
        public List<Capitulo> Capitulos
        {
            get
            {
                List<Capitulo> caps = new List<Capitulo>(CapitulosDescargados);
                caps.AddRange(CapitulosImpresos);
                caps.AddRange(CapitulosPorDescargar);
                caps.Sort(new ComparerOrdenadorCapitulos());
                return caps;
            }
        }


        /// <summary>
        /// Define si esta novela está 100% descargada.
        /// </summary>
        public bool EstoyCompleta
        {
            get
            {
                return CantidadCapitulosDescargados == LinksDeCapitulos.Count;
            }
        }

        /// <summary>
        /// Define el % de descarga de la novela.
        /// </summary>
        public int PorcentajeDescarga => CapitulosDescargados.Count * 100 / CantidadLinks;

        public string Autor => throw new NotImplementedException();

        public bool HistoriaCompleta => throw new NotImplementedException();

        public bool TraduccionCompleta => throw new NotImplementedException();

        public float Review => throw new NotImplementedException();

        public float CantidadReviews => throw new NotImplementedException();

        public List<string> Generos => throw new NotImplementedException();

        int INovela.CantidadReviews => throw new NotImplementedException();


        #endregion


        #region Metodos

        /// <summary>
        /// Agrega un capitulo a la novela.
        /// </summary>
        /// <param name="capituloNuevo"></param>
        public void CapituloFueDescargado(Capitulo capituloNuevo)
        {
            CapitulosPorDescargar.Remove(capituloNuevo);
            CapitulosDescargados.Add(capituloNuevo);
        }


        /// <summary>
        /// Informa que X capitulo ha sido impreso.
        /// </summary>
        /// <param name="capitulo"></param>
        public void CapituloFueImpreso(Capitulo capitulo)
        {
            CapitulosDescargados.Remove(capitulo);
            CapitulosImpresos.Add(capitulo);
        }

        #endregion
    }
}