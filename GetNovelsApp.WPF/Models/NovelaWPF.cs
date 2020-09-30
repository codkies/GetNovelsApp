using System;
using System.Collections.Generic;
using System.Drawing;
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
        public NovelaWPF(List<Capitulo> capitulos, InformacionNovelaDB dbInfo)
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

            ID = dbInfo.ID;
            Titulo = dbInfo.Titulo;
            LinkPrincipal = new Uri(dbInfo.LinkPrincipal);
        }

        private void OrdenaListas()
        {
            CapitulosDescargados.Sort(new ComparerOrdenadorCapitulos());
            CapitulosPorDescargar.Sort(new ComparerOrdenadorCapitulos());
        }


        #region Identificadores y fields clave

        private string titulo;
        private List<Capitulo> capitulosDescargados;
        private List<Capitulo> capitulosImpresos;
        private List<Capitulo> capitulosPorDescargar;
        private Uri linkPrincipal;
        private List<Uri> linksDeCapitulos;
        private string sipnosis;
        private Image imagen;

        public Image Imagen
        {
            get => imagen;
            set => OnPropertyChanged(ref imagen, value);
        }

        public string Sipnosis
        {
            get => sipnosis;
            set => OnPropertyChanged(ref sipnosis, value);
        }

        public string Titulo
        {
            get => titulo;
            set => OnPropertyChanged(ref titulo, value);
        }


        public int ID { get; private set; }

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

        public Uri LinkPrincipal
        {
            get => linkPrincipal;
            set => OnPropertyChanged(ref linkPrincipal, value);
        }

        public List<Uri> LinksDeCapitulos
        {
            get => linksDeCapitulos;
            set => OnPropertyChanged(ref linksDeCapitulos, value);
        }


        #endregion


        #region Externos (por implementar)

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

        #endregion


        #region Cambio de estado (por implementar)

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
