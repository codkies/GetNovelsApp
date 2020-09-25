using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GetNovelsApp.Core.Conexiones;

namespace GetNovelsApp.Core.Modelos
{
    public class Novela
    {
        public Novela(string link, string FolderPath, int EmpezarEn)
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
        public readonly string LinkPaginaPrincipal;


        /// <summary>
        /// Lista de todos los links de los capitulos
        /// </summary>
        public readonly List<string> LinksDeCapitulos;


        /// <summary>
        /// Link del primer capitulo.
        /// </summary>
        public readonly string PrimerLink;


        /// <summary>
        /// Link del ultimo capitulo.
        /// </summary>
        public readonly string UltimoLink;
        

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
        /// Capitulos presentes en esta novela que no han sido metidos en el PDF
        /// </summary>
        public IEnumerable<Capitulo> CapitulosSinImprimir => new List<Capitulo>(_CapitulosSinImprimir);


        /// <summary>
        /// Capitulos presentes en esta novela que ya han sido metidos en el PDF
        /// </summary>
        public IEnumerable<Capitulo> CapitulosImpresos => new List<Capitulo>(_CapitulosImpresos);        


        /// <summary>
        /// Define si esta novela tiene capitulos por imprimir
        /// </summary>
        public bool TengoCapitulosPorImprimir => _CapitulosSinImprimir.Count > 0;

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


        /// <summary>
        /// Toma un capitulo de los capitulos sin imprimir.
        /// </summary>
        /// <param name="indexCapitulo">Index del capitulo en la lista de capitulos sin imprimir.</param>
        /// <returns></returns>
        public Capitulo ConsigueCapituloSinImprimir(int indexCapitulo)
        {
            if (_CapitulosSinImprimir.Count < indexCapitulo)
                throw new IndexOutOfRangeException("index capitulo");

            return _CapitulosSinImprimir[indexCapitulo];
        }
        
    }
}