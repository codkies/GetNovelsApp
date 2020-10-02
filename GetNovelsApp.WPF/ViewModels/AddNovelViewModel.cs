using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.WPF.Utilidades;
using GetNovelsApp.WPF.Views.TEst;


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
            Ejecuta_BuscaLink = new RelayCommand<string>(ObtenInfoNovela, EsLink);
            Ejecuta_GuardaNovela = new RelayCommand(GuardaNovela, PuedeGuardarNovela);
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


        #region Comandos

        public ICommand Ejecuta_BuscaLink { get; private set; }
        public ICommand Ejecuta_GuardaNovela { get; private set; }


        #endregion

        #region Comandos (metodos)

        public void ObtenInfoNovela(string link)
        {
            Uri Link = new Uri(link);
            InfoNovela = ManipuladorDeLinks.EncuentraInformacionNovela(Link);

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


        public bool EsLink(string posibleLink)
        {
            return Uri.TryCreate(posibleLink, uriKind: UriKind.Absolute, out _);
        } 



        public void GuardaNovela()
        {
            Archivador.MeteNovelaDB(InfoNovela);
        }

        public bool PuedeGuardarNovela()
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

        private string DescargaImagen(string url)
        {
            string filePath = System.IO.Path.GetFileName(url);

            System.Net.WebClient cln = new System.Net.WebClient();
            filePath = $@"C:\NovelApp\{filePath}";

            cln.DownloadFile(url, filePath);
            return filePath;
        }


    }
}
