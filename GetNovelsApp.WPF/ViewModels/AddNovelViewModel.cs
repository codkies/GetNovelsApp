using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.WPF.Utilidades;


namespace GetNovelsApp.WPF.ViewModels
{
    /// <summary>
    /// Controlador de la vista para agregar una novela nueva.
    /// </summary>
    public class AddNovelViewModel : ObservableObject
    {
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
        private int cantidadCapitulos;
        private string sipnosis;
        private List<string> tags;
        private Uri linkNovela;
        private string imagenPath;

        #endregion

        public string Titulo { get => titulo; set => OnPropertyChanged(ref titulo, value); }
        public int CantidadCapitulos { get => cantidadCapitulos; set => OnPropertyChanged(ref cantidadCapitulos, value); }
        public string Sipnosis { get => sipnosis; set => OnPropertyChanged(ref sipnosis, value); }
        public List<string> Tags { get => tags; set => OnPropertyChanged(ref tags, value); }

        public Uri LinkNovela { get => linkNovela; set => OnPropertyChanged(ref linkNovela, value); }

        public string PathImagenNovela { get => imagenPath; set => OnPropertyChanged(ref imagenPath, value); }


        #endregion


        #region Buscar link

        public RelayCommand<string> Ejecuta_BuscaLink { get; private set; }


        public async void BuscaLink(string link)
        {
            Uri Link = new Uri(link);
            InfoNovela = await Task.Run( ()=> ManipuladorDeLinks.EncuentraInformacionNovela(Link));

            //Descarga imagen
            Task.Run(() => ActualizaImagen(InfoNovela.Imagen.ToString()));
            //Revisa si esta novela está en la DB.
            Task.Run(() => NovelaYaEstaEnDB = Archivador.NovelaExisteEnDB(Link));

            Titulo = InfoNovela.Titulo;
            CantidadCapitulos = InfoNovela.LinksDeCapitulos.Count;
            Sipnosis = InfoNovela.Sipnosis;
            Tags = InfoNovela.Tags;
            LinkNovela = Link;
        }


        public bool Puede_BuscaLink(string posibleLink)
        {
            return Uri.TryCreate(posibleLink, uriKind: UriKind.Absolute, out _);
        }


        #endregion


        #region Guardar novela

        public RelayCommand<Window> Ejecuta_GuardaNovela { get; private set; }


        public void GuardarNovela(Window window)
        {
            Archivador.MeteNovelaDBAsync(InfoNovela);
            window.Close();
        }


        public bool Puede_GuardarNovela(Window window)
        {
            return (!NovelaYaEstaEnDB) & (InfoNovela != null) ;
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
