using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;


namespace GetNovelsApp.WPF.ViewModels
{
    /// <summary>
    /// Controlador de la vista para agregar una novela nueva.
    /// </summary>
    public class AddNovelViewModel : ObservableObject, IReportero
    {
        public string Nombre => "AddNovelView";

        public AddNovelViewModel()
        {
            Archivador = new Archivador();
            Ejecuta_BuscaLink = new RelayCommand<string>(BuscaLink, Puede_BuscaLink);
            Ejecuta_GuardaNovela = new RelayCommand<Window>(GuardarNovela, Puede_GuardarNovela);
        }

        InformacionNovelaOnline InfoNovela;
        bool NovelaYaEstaEnDB;
        Archivador Archivador;

        #region Propieades

        #region backing fields
        private string titulo;
        private string autor;
        private string review;
        private int cantidadCapitulos;
        private string sipnosis;
        private List<string> tags;
        private List<string> generos;
        private Uri linkNovela;
        private string imagenPath;
        private string contenidoBotonGuardado;
        private string contenidoBotonBusqueda;

        #endregion

        public string Titulo { get => titulo; set => OnPropertyChanged(ref titulo, value); }
        public string Autor { get => autor; set => OnPropertyChanged(ref autor, value); }
        public string Review { get => review; set => OnPropertyChanged(ref review, value); }
        public int CantidadCapitulos { get => cantidadCapitulos; set => OnPropertyChanged(ref cantidadCapitulos, value); }
        public string Sipnosis { get => sipnosis; set => OnPropertyChanged(ref sipnosis, value); }
        public List<string> Tags { get => tags; set => OnPropertyChanged(ref tags, value); }

        public List<string> Generos { get => generos; set => OnPropertyChanged(ref generos, value); }

        public Uri LinkNovela { get => linkNovela; set => OnPropertyChanged(ref linkNovela, value); }

        public string PathImagenNovela { get => imagenPath; set => OnPropertyChanged(ref imagenPath, value); }

        public int Progreso { get => progreso; set => OnPropertyChanged(ref progreso, value); }

        public string ContenidoBotonGuardado { get => contenidoBotonGuardado; set => OnPropertyChanged(ref contenidoBotonGuardado, value); }
        public string ContenidoBotonBusqueda { get => contenidoBotonGuardado; set => OnPropertyChanged(ref contenidoBotonGuardado, value); }
        #endregion


        #region Buscar link

        public RelayCommand<string> Ejecuta_BuscaLink { get; private set; }

        private string LinkViejo = "";
        private int progreso;

        public async void BuscaLink(string link)
        {
            ContenidoBotonBusqueda = "Buscando";
            LinkViejo = link;
            Uri Link = new Uri(link);

            InfoNovela = await Task.Run(() => ManipuladorDeLinks.EncuentraInformacionNovela(Link));

            ContenidoBotonBusqueda = "Busca";
            //Descarga imagen
            Task.Run(() => ActualizaImagen(InfoNovela.Imagen.ToString()));
            //Revisa si esta novela está en la DB.
            Task.Run(() => NovelaYaEstaEnDB = Archivador.NovelaExisteEnDB(Link));

            Titulo = InfoNovela.Titulo;
            CantidadCapitulos = InfoNovela.LinksDeCapitulos.Count;
            Sipnosis = InfoNovela.Sipnosis;
            Tags = InfoNovela.Tags;
            LinkNovela = Link;
            Generos = InfoNovela.Generos;
            Autor = InfoNovela.Autor;
            Review = InfoNovela.Review.ToString();
        }


        public bool Puede_BuscaLink(string v)
        {
            string posibleLink = v == null ? string.Empty : v;
            return Uri.TryCreate(posibleLink, uriKind: UriKind.Absolute, out _) & !posibleLink.Equals(LinkViejo);
        }


        #endregion


        #region Guardar novela

        public RelayCommand<Window> Ejecuta_GuardaNovela { get; private set; }

        bool Ejecutando = false;
        public async void GuardarNovela(Window window)
        {
            IReporte r = GetNovelsFactory.FabricaReporteNovela(0, 0, "Guardando", this, InfoNovela.Titulo);
            
            ManagerTareas.MuestraReporte(r);
            Progreso progresoGuardado = new Progreso();
            progresoGuardado.ProgressChanged += ProgresoGuardado_ProgressChanged;
            ContenidoBotonGuardado = "Guardando...";
            Ejecutando = true;
            Task.Run( ()=> FinProceso(progresoGuardado));
            //window.Close();
        }

        private async Task FinProceso(Progreso progresoGuardado)
        {
            await Archivador.MeteNovelaDBAsync(InfoNovela, progresoGuardado);
            Ejecutando = false;
            Titulo = "";
            Review = "";
            Autor = "";
            Progreso = 0;
            PathImagenNovela = string.Empty;
            LinkNovela = null;
            Generos = new List<string>();
            Tags = new List<string>();
            Sipnosis = "";
            CantidadCapitulos = 0;
            ContenidoBotonGuardado = "Guarda Novela";
        }

        private void ProgresoGuardado_ProgressChanged(object sender, IReporte e)
        {
            ManagerTareas.ActualizaReporte(e);
            Progreso = e.PorcentajeDeCompletado;
        }


        public bool Puede_GuardarNovela(Window window)
        {
            return (!NovelaYaEstaEnDB) & (InfoNovela != null) & !Ejecutando;
        }


        #endregion



        /// <summary>
        /// Actualiza la imagen a mostrar.
        /// </summary>
        private void ActualizaImagen(string url)
        {
            PathImagenNovela = EncontradorImagen.DescargaImagen(url);
        }
    }


}
