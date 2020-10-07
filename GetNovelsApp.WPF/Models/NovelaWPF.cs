using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.Models
{
    /// <summary>
    /// Novela version WPF
    /// </summary>
    public class NovelaWPF : ObservableObject, INovela
    {
        #region Setup

        public NovelaWPF(List<Capitulo> capitulos, InformacionNovelaDB dbInfo)
        {
            OrganizaCapitulos(capitulos);

            ID = dbInfo.ID;
            Titulo = dbInfo.Titulo;
            LinkPrincipal = new Uri(dbInfo.LinkPrincipal);

            if (dbInfo.Tags != null)
                Tags = ManipuladorStrings.TagsEnLista(dbInfo.Tags);

            Sipnosis = dbInfo.Sipnosis;
            if (dbInfo.Imagen != null)
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


        #region Identificadores y fields clave

        #region backing prop fields
        //Info
        private string titulo;
        private string sipnosis;
        private Uri linkPrincipal;
        private List<string> tags = new List<string>();
        private Uri imagenLink;

        //Caps
        private List<Uri> linksDeCapitulos = new List<Uri>();
        private List<Capitulo> capitulosDescargados = new List<Capitulo>();
        private List<Capitulo> capitulosImpresos = new List<Capitulo>();
        private List<Capitulo> capitulosPorDescargar = new List<Capitulo>();

        private string autor;
        private bool historiaCompleta;
        private bool traduccionCompleta;
        private float review;
        private int cantidadReviews;
        private List<string> generos;
        #endregion

        #region new fields

        public string Autor { get => autor; set => OnPropertyChanged(ref titulo, value); }


        public bool HistoriaCompleta { get => historiaCompleta; set => OnPropertyChanged(ref historiaCompleta, value); }


        public bool TraduccionCompleta { get => traduccionCompleta; set => OnPropertyChanged(ref traduccionCompleta, value); }


        public float Review { get => review; set => OnPropertyChanged(ref review, value); }


        public int CantidadReviews { get => cantidadReviews; set => OnPropertyChanged(ref cantidadReviews, value); }


        public List<string> Generos { get => generos; set => OnPropertyChanged(ref generos, value); }

        #endregion

        //Info
        public string Titulo
        {
            get => titulo;
            set => OnPropertyChanged(ref titulo, value);
        }


        public List<string> Tags
        {
            get => tags;
            set => OnPropertyChanged(ref tags, value);
        }


        public Uri ImagenLink
        {
            get => imagenLink;
            set => OnPropertyChanged(ref imagenLink, value);
        }


        public string Sipnosis
        {
            get => sipnosis;
            set => OnPropertyChanged(ref sipnosis, value);
        }


        public int ID { get; private set; }


        public Uri LinkPrincipal
        {
            get => linkPrincipal;
            set => OnPropertyChanged(ref linkPrincipal, value);
        }


        //Caps
        public List<Capitulo> CapitulosDescargados
        {
            get => capitulosDescargados;
            set => OnPropertyChanged(ref capitulosDescargados, value);
        }

        public List<Capitulo> CapitulosImpresos
        {
            get => capitulosImpresos;
            set => OnPropertyChanged(ref capitulosImpresos, value);
        }

        public List<Capitulo> CapitulosPorDescargar
        {
            get => capitulosPorDescargar;
            set => OnPropertyChanged(ref capitulosPorDescargar, value);
        }

        public List<Uri> LinksDeCapitulos
        {
            get => linksDeCapitulos;
            set => OnPropertyChanged(ref linksDeCapitulos, value);
        }


        #endregion


        #region Externos

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


        /// <summary>
        /// Usado por el XAML. Do not delete.
        /// </summary>
        public string PathImagen => EncontradorImagen.DescargaImagen(ImagenLink);

        #endregion


        #region Cambio de estado

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
