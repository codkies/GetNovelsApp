using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Utilidades;
using iText.Layout.Properties;

namespace GetNovelsApp.Core.Modelos
{
    /// <summary>
    /// Modelo que utiliza la app para conseguir capitulos y ordenarlos.
    /// </summary>
    public class NovelaRuntimeModel : INovela
    {
        public NovelaRuntimeModel(List<Capitulo> capitulos, NovelaDBModel dbInfo)
        {
            foreach (Capitulo c in capitulos)
            {
                _LinksDeCapitulos.Add(c.Link);

                if (string.IsNullOrEmpty(c.Texto) == false)
                {
                    _CapitulosDescargados.Add(c);
                }
                else
                {
                    _CapitulosPorDescargar.Add(c);
                }
            }
            OrdenaListas();
            this.dbInfo = dbInfo;
            ID = dbInfo.ID;
        }

        private void OrdenaListas()
        {
            _CapitulosDescargados.Sort(new ComparerOrdenadorCapitulos());
            _CapitulosPorDescargar.Sort(new ComparerOrdenadorCapitulos());
        }


        #region Fields

        private List<Capitulo> _CapitulosPorDescargar = new List<Capitulo>();

        private List<Capitulo> _CapitulosDescargados = new List<Capitulo>();

        private List<Capitulo> _CapitulosImpresos = new List<Capitulo>();

        private List<Uri> _LinksDeCapitulos = new List<Uri>();

        NovelaDBModel dbInfo;

        /// <summary>
        /// ID en DB.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo => dbInfo.Titulo;

        /// <summary>
        /// Link a su pagina principal de la novela
        /// </summary>
        public Uri LinkPrincipal => new Uri(dbInfo.LinkPrincipal);


        #endregion


        #region Props

        /// <summary>
        /// Define si esta novela tiene capitulos por imprimir
        /// </summary>
        public bool TengoCapitulosPorImprimir => _CapitulosDescargados.Count > 0;

        /// <summary>
        /// Lista de todos los links de los capitulos
        /// </summary>
        public List<Uri> LinksDeCapitulos => _LinksDeCapitulos;

        /// <summary>
        /// Capitulos presentes en esta novela que no han sido metidos en el PDF
        /// </summary>
        public ReadOnlyCollection<Capitulo> CapitulosDescargados => _CapitulosDescargados.AsReadOnly();


        /// <summary>
        /// Capitulos presentes en esta novela que ya han sido metidos en el PDF
        /// </summary>
        public ReadOnlyCollection<Capitulo> CapitulosImpresos => _CapitulosImpresos.AsReadOnly();



        public ReadOnlyCollection<Capitulo> CapitulosPorDescargar => _CapitulosPorDescargar.AsReadOnly();


        /// <summary>
        /// Define la cantidad de capitulos que esta novela tiene que no se han impreso.
        /// </summary>
        public int CantidadCapitulosDescargados => _CapitulosDescargados.Count;


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
                List<Capitulo> caps = new List<Capitulo>(_CapitulosDescargados);
                caps.AddRange(_CapitulosImpresos);
                caps.AddRange(_CapitulosPorDescargar);
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


        /// <summary>
        /// Agrega un capitulo a la novela.
        /// </summary>
        /// <param name="capituloNuevo"></param>
        public void CapituloFueDescargado(Capitulo capituloNuevo)
        {
            _CapitulosPorDescargar.Remove(capituloNuevo);
            _CapitulosDescargados.Add(capituloNuevo);
        }


        /// <summary>
        /// Informa que X capitulo ha sido impreso.
        /// </summary>
        /// <param name="capitulo"></param>
        public void CapituloFueImpreso(Capitulo capitulo)
        {
            _CapitulosDescargados.Remove(capitulo);
            _CapitulosImpresos.Add(capitulo);
        }

    }
}