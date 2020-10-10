using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.Models
{
    /// <summary>
    /// Novela version WPF
    /// </summary>
    public class NovelaWPF : ObservableObject, INovela<ObservableCollection<Capitulo>, ObservableCollection<string>, ObservableCollection<Uri>>
    {
        #region Setup

        public NovelaWPF(List<Capitulo> capitulos, InformacionNovelaDB dbInfo)
        {
            OrganizaCapitulos(capitulos);

            ID = dbInfo.ID;
            Titulo = dbInfo.Titulo;
            Autor = dbInfo.Autor;

            HistoriaCompleta = dbInfo.EstadoHistoria == EstadoHistoria.Completa;
            TraduccionCompleta = dbInfo.EstadoTraduccion == EstadoTraduccion.Completa;

            Review = dbInfo.Review;
            CantidadReviews = dbInfo.CantidadReviews;

            LinkPrincipal = new Uri(dbInfo.LinkPrincipal);

            if (dbInfo.Tags != null)
                Tags = new ObservableCollection<string>(ManipuladorStrings.TagsEnLista(dbInfo.Tags));
            if (dbInfo.Generos != null)
                Generos = new ObservableCollection<string>(ManipuladorStrings.TagsEnLista(dbInfo.Generos));

            Sipnosis = dbInfo.Sipnosis;
            if (dbInfo.Imagen != null)
                ImagenLink = new Uri(dbInfo.Imagen);
        }

        private void OrganizaCapitulos(List<Capitulo> capitulos)
        {
            var Links = new List<Uri>();
            var Descargados = new List<Capitulo>();
            var PorDescargar = new List<Capitulo>();

            foreach (Capitulo c in capitulos)
            {
                Links.Add(c.Link);

                if (string.IsNullOrEmpty(c.Texto) == false)
                {
                    Descargados.Add(c);
                }
                else
                {
                    PorDescargar.Add(c);
                }
            }


            Descargados.Sort(new ComparerOrdenadorCapitulos());
            PorDescargar.Sort(new ComparerOrdenadorCapitulos());

            LinksDeCapitulos = new ObservableCollection<Uri>(Links);
            CapitulosPorDescargar = new ObservableCollection<Capitulo>(PorDescargar);
            CapitulosDescargados = new ObservableCollection<Capitulo>(Descargados);
        }


        #endregion


        #region Identificadores y fields clave

        #region backing prop fields
        //Info
        private string titulo;
        private string sipnosis;
        private Uri linkPrincipal;
        private ObservableCollection<string> tags = new ObservableCollection<string>();
        private Uri imagenLink;

        //Caps
        private ObservableCollection<Uri> linksDeCapitulos = new ObservableCollection<Uri>();
        private ObservableCollection<Capitulo> capitulosDescargados = new ObservableCollection<Capitulo>();
        private ObservableCollection<Capitulo> capitulosImpresos = new ObservableCollection<Capitulo>();
        private ObservableCollection<Capitulo> capitulosPorDescargar = new ObservableCollection<Capitulo>();

        private string autor;
        private bool historiaCompleta;
        private bool traduccionCompleta;
        private float review;
        private int cantidadReviews;
        private ObservableCollection<string> generos;
        #endregion

        #region Fields para WPF

        public ObservableCollection<string> Generos
        {
            get => generos;
            set => OnPropertyChanged(ref generos, value);
        }

        public ObservableCollection<string> Tags
        {
            get => tags;
            set => OnPropertyChanged(ref tags, value);
        }

        public ObservableCollection<Capitulo> CapitulosDescargados
        {
            get => capitulosDescargados;
            set => OnPropertyChanged(ref capitulosDescargados, value);
        }

        public ObservableCollection<Capitulo> CapitulosImpresos
        {
            get => capitulosImpresos;
            set => OnPropertyChanged(ref capitulosImpresos, value);
        }

        public ObservableCollection<Capitulo> CapitulosPorDescargar
        {
            get => capitulosPorDescargar;
            set => OnPropertyChanged(ref capitulosPorDescargar, value);
        }

        public ObservableCollection<Uri> LinksDeCapitulos
        {
            get => linksDeCapitulos;
            set => OnPropertyChanged(ref linksDeCapitulos, value);
        }



        #endregion

        public string Autor { get => autor; set => OnPropertyChanged(ref autor, value); }


        public bool HistoriaCompleta { get => historiaCompleta; set => OnPropertyChanged(ref historiaCompleta, value); }


        public bool TraduccionCompleta { get => traduccionCompleta; set => OnPropertyChanged(ref traduccionCompleta, value); }


        public float Review { get => review; set => OnPropertyChanged(ref review, value); }


        public int CantidadReviews { get => cantidadReviews; set => OnPropertyChanged(ref cantidadReviews, value); }

        //Info
        public string Titulo
        {
            get => titulo;
            set => OnPropertyChanged(ref titulo, value);
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
        public ObservableCollection<Capitulo> Capitulos
        {
            get
            {
                List<Capitulo> caps = new List<Capitulo>(CapitulosDescargados);
                caps.AddRange(CapitulosImpresos);
                caps.AddRange(CapitulosPorDescargar);
                caps.Sort(new ComparerOrdenadorCapitulos());

                return new ObservableCollection<Capitulo>(caps);
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
