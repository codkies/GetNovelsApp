using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using GetNovelsApp.Core.Conexiones;

namespace GetNovelsApp.Core.Modelos
{
    public class Novela
    {
        public Novela(Uri link, string FolderPath, int EmpezarEn)
        {
            this.EmpezarEn = EmpezarEn;
            CarpetaPath = FolderPath;
            InformacionNovela info = ManipuladorDeLinks.EncuentraInformacionNovela(link);

            Titulo = info.Titulo;
            CarpetaPath += $"\\{Titulo}\\";
            // CarpetaPath = FolderPath +  $"\\{Titulo}\\";
            LinkPaginaPrincipal = info.LinkPaginaPrincipal;

            LinksDeCapitulos = info.LinksDeCapitulos;
            PrimerLink = info.PrimerLink;
            UltimoLink = info.UltimoLink;

            UltimoNumeroCapitulo = info.UltimoCapitulo;
            PrimerNumeroCapitulo = info.PrimerCapitulo;
        }


        #region Fields

        private List<Capitulo> _CapitulosSinImprimir = new List<Capitulo>();

        private List<Capitulo> _CapitulosImpresos = new List<Capitulo>();


        /// <summary>
        /// Titulo de la novela
        /// </summary>
        public readonly string Titulo;


        /// <summary>
        /// Direccion a la carpeta donde se guardará la novela
        /// </summary>
        public readonly string CarpetaPath;


        /// <summary>
        /// Link a su pagina principal de la novela
        /// </summary>
        public readonly Uri LinkPaginaPrincipal;


        /// <summary>
        /// Lista de todos los links de los capitulos
        /// </summary>
        public readonly List<Uri> LinksDeCapitulos;


        /// <summary>
        /// Link del primer capitulo.
        /// </summary>
        public readonly Uri PrimerLink;


        /// <summary>
        /// Link del ultimo capitulo.
        /// </summary>
        public readonly Uri UltimoLink;
        

        /// <summary>
        /// Index de los links donde el usuario quiere comenzar.
        /// </summary>
        public readonly int EmpezarEn;


        /// <summary>
        /// Capitulo final de la novela. Encotrado segun el link Original.
        /// </summary>
        public readonly float UltimoNumeroCapitulo;


        /// <summary>
        /// Primer capitulo de la novela. Encotrado segun el link Original.
        /// </summary>
        public readonly float PrimerNumeroCapitulo;

        #endregion


        #region Props

        /// <summary>
        /// Define si esta novela tiene capitulos por imprimir
        /// </summary>
        private bool TengoCapitulosPorImprimir => _CapitulosSinImprimir.Count > 0;

        /// <summary>
        /// Capitulos presentes en esta novela que no han sido metidos en el PDF
        /// </summary>
        public ReadOnlyCollection<Capitulo> CapitulosSinImprimir => _CapitulosSinImprimir.AsReadOnly();


        /// <summary>
        /// Capitulos presentes en esta novela que ya han sido metidos en el PDF
        /// </summary>
        public ReadOnlyCollection<Capitulo> CapitulosImpresos => _CapitulosImpresos.AsReadOnly();        


        /// <summary>
        /// Define la cantidad de capitulos que esta novela tiene que no se han impreso.
        /// </summary>
        public int CantidadCapitulosPorImprimir => _CapitulosSinImprimir.Count;


        /// <summary>
        /// Cantidad total de links que contiene esta novela.
        /// </summary>
        public int CantidadLinks => LinksDeCapitulos.Count;

        #endregion


        /// <summary>
        /// Agrega un capitulo a la novela.
        /// </summary>
        /// <param name="capituloNuevo"></param>
        public void AgregaCapitulo(Capitulo capituloNuevo)
        {
            _CapitulosSinImprimir.Add(capituloNuevo);
        }


        /// <summary>
        /// Informa que X capitulo ha sido impreso.
        /// </summary>
        /// <param name="capitulo"></param>
        public void CapituloFueImpreso(Capitulo capitulo)
        {
            _CapitulosSinImprimir.Remove(capitulo);
            _CapitulosImpresos.Add(capitulo);
        }

    }
}