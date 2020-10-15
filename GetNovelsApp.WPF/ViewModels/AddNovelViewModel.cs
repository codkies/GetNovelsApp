using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.Modelos;
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
        InformacionNovelaOnline InfoNovela;
        bool NovelaYaEstaEnDB;
        Archivador ar;
        List<string> dominiosSoportados = new List<string>();

        public AddNovelViewModel()
        {
            ar = new Archivador();

            GetNovelsEvents.WebsitesCambiaron += GetNovelsEvents_WebsitesCambiaron;
            Ejecuta_BuscaLink = new RelayCommand<string>(BuscaLink, Puede_BuscaLink);
            Ejecuta_GuardaNovela = new RelayCommand<Window>(GuardarNovela, Puede_GuardarNovela);

            GetNovelsEvents_WebsitesCambiaron();
        }

        

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
        private ObservableCollection<string> links = new ObservableCollection<string>();

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
        public string ContenidoBotonBusqueda { get => contenidoBotonBusqueda; set => OnPropertyChanged(ref contenidoBotonBusqueda, value); }

        public ObservableCollection<string> Links { get => links;

            set
            {
                OnPropertyChanged(ref links, value);
                ActualizaCantidadLinks();
            }
        }

        
        #endregion


        #region Buscar link

        public RelayCommand<string> Ejecuta_BuscaLink { get; private set; }

        /// <summary>
        /// usado para comparar el link nuevo y el viejo 
        /// </summary>
        private string LinkViejo = "";
        private int progreso;
        private bool Buscando = false;
        public async void BuscaLink(string link)
        {
            ContenidoBotonBusqueda = "Buscando";            
            Buscando = true;
            LinkViejo = link;
            Uri Link = new Uri(link);

            InfoNovela = await Task.Run(() => ManipuladorDeLinks.EncuentraInformacionNovela(Link));

            ContenidoBotonBusqueda = "Busca";
            Buscando = false;
            //Descarga imagen
            Task.Run(() => ActualizaImagen(InfoNovela.Imagen.ToString()));
            //Revisa si esta novela está en la DB.
            Task.Run(() => NovelaYaEstaEnDB = ar.NovelaExisteEnDB(Link));

            Titulo = InfoNovela.Titulo;            
            Sipnosis = InfoNovela.Sipnosis;
            Tags = InfoNovela.Tags;
            LinkNovela = Link;
            Generos = InfoNovela.Generos;
            Autor = InfoNovela.Autor;
            Review = InfoNovela.Review.ToString();

            var x = new List<string>();
            foreach (Uri uri in InfoNovela.LinksDeCapitulos)
            {
                x.Add(uri.ToString());
            }
            Links = new ObservableCollection<string>(x);
            
        }


        public bool Puede_BuscaLink(string v)
        {
            if(string.IsNullOrEmpty(v))
            {
                ContenidoBotonBusqueda = "Busca";
                return false;
            }

            if(v == LinkViejo)
            {
                ContenidoBotonBusqueda = "---";
                return false;
            }
            
            bool esUnLink = Uri.TryCreate(v, uriKind: UriKind.Absolute, out Uri Link);
            if (esUnLink == false)
            {
                ContenidoBotonBusqueda = "No soportado";
                return false;
            }

            bool dominioSoportado = dominiosSoportados.Contains(Link.IdnHost);
            if (dominioSoportado == false)
            {
                ContenidoBotonBusqueda = "No soportado";
                return false;
            }

            ContenidoBotonBusqueda = "Busca";

            return Buscando == false;
        }


        #endregion


        #region Guardar novela

        public RelayCommand<Window> Ejecuta_GuardaNovela { get; private set; }

        bool Ejecutando = false;        

        public async void GuardarNovela(Window window)
        {
            IReporte r = GetNovelsFactory.FabricaReporteNovela(100, 0, "Guardando", this, InfoNovela.Titulo);

            ManagerTareas.MuestraReporte(r);
            Progreso progresoGuardado = new Progreso();
            progresoGuardado.ProgressChanged += ProgresoGuardado_ProgressChanged;
            ContenidoBotonGuardado = "Guardando...";
            Ejecutando = true;
            Task.Run(() => FinProceso(progresoGuardado));
            //window.Close();
        }

        private async Task FinProceso(Progreso progresoGuardado)
        {
            var x = new List<Uri>();
            foreach (var link in Links)
            {
                x.Add(new Uri(link));
            }
            InfoNovela.LinksDeCapitulos = x;            

            await ar.MeteNovelaDBAsync(InfoNovela, progresoGuardado);
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
            Links.Clear();
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


        #region Event handlers


        private void GetNovelsEvents_WebsitesCambiaron()
        {
            foreach (IPath website in GetNovelsConfig.WebsitesSoportados)
            {
                dominiosSoportados.Add(website.Dominio);
            }
        }

        #endregion



        /// <summary>
        /// Actualiza la imagen a mostrar.
        /// </summary>
        private void ActualizaImagen(string url)
        {
            PathImagenNovela = EncontradorImagen.DescargaImagen(url);
        }

        private void ActualizaCantidadLinks()
        {
            CantidadCapitulos = Links.Count;
        }
    }


}
