using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using GetNovelsApp.Core.Conexiones;

namespace GetNovelsApp.Core.Modelos
{
    public class Novela
    {
        public Novela(InformacionNovela info, int ID)
        {
            MiInfo = info;
            this.ID = ID;
        }       

        public Novela(InformacionNovela info, int ID, List<Capitulo> Capitulos)
        {
            _CapitulosDescargados = new List<Capitulo>(Capitulos);
            MiInfo = info;
            this.ID = ID;
        }


        #region Fields

        private List<Capitulo> _CapitulosDescargados = new List<Capitulo>();

        private List<Capitulo> _CapitulosImpresos = new List<Capitulo>();

        InformacionNovela MiInfo;

        /// <summary>
        /// ID en DB.
        /// </summary>
        public int ID { get; private set; }

        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public string Titulo => MiInfo.Titulo;

        /// <summary>
        /// Link a su pagina principal de la novela
        /// </summary>
        public Uri LinkPrincipal => MiInfo.LinkPrincipal;


        /// <summary>
        /// Lista de todos los links de los capitulos
        /// </summary>
        public List<Uri> LinksDeCapitulos => MiInfo.LinksDeCapitulos;


        /// <summary>
        /// Link del primer capitulo.
        /// </summary>
        public Uri PrimerLink => LinksDeCapitulos.First();


        /// <summary>
        /// Link del ultimo capitulo.
        /// </summary>
        public Uri UltimoLink => LinksDeCapitulos.Last();

        /// <summary>
        /// Capitulo final de la novela. Encotrado segun el link Original.
        /// </summary>
        public float UltimoNumeroCapitulo => ManipuladorDeLinks.EncuentraInformacionCapitulo(PrimerLink).NumeroCapitulo;


        /// <summary>
        /// Primer capitulo de la novela. Encotrado segun el link Original.
        /// </summary>
        public float PrimerNumeroCapitulo => ManipuladorDeLinks.EncuentraInformacionCapitulo(UltimoLink).NumeroCapitulo;

        #endregion


        #region Props

        /// <summary>
        /// Define si esta novela tiene capitulos por imprimir
        /// </summary>
        public bool TengoCapitulosPorImprimir => _CapitulosDescargados.Count > 0;

        /// <summary>
        /// Capitulos presentes en esta novela que no han sido metidos en el PDF
        /// </summary>
        public ReadOnlyCollection<Capitulo> CapitulosDescargados => _CapitulosDescargados.AsReadOnly();


        /// <summary>
        /// Capitulos presentes en esta novela que ya han sido metidos en el PDF
        /// </summary>
        public ReadOnlyCollection<Capitulo> CapitulosImpresos => _CapitulosImpresos.AsReadOnly();        


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
                return caps;
            }
        }

        #endregion


        /// <summary>
        /// Agrega un capitulo a la novela.
        /// </summary>
        /// <param name="capituloNuevo"></param>
        public void CapituloFueDescargado(Capitulo capituloNuevo)
        {
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