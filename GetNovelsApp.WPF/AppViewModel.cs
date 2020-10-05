using GetNovelsApp.Core;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;
using GetNovelsApp.WPF.ViewModels;

namespace GetNovelsApp.WPF
{
    public class AppViewModel : ObservableObject
    {
        #region Private fields

        public static GetNovels GetNovels { get; private set; }


        private static bool Inicializado = false;

        #endregion

        #region Setup

        public AppViewModel()
        {
            InicializaApp();
            //Descargas 
            GetNovelsWPFEvents.CambiaViewModel += GetNovelsWPFEvents_CambiaViewModel;

            Command_VeBiblioteca = new RelayCommand(VeBiblioteca, Puedo_VeBibliteca);
            Command_VeConfiguracion = new RelayCommand(VeConfiguracion);
            Command_VeDescargas = new RelayCommand(VeDescargas);

            VeDescargas();
            //VeBiblioteca();
        }

        private void GetNovelsWPFEvents_CambiaViewModel(object obj)
        {
            CurrentView = obj;
        }


        /// <summary>
        /// Toma referencias de GetNovels y GetNovelsConfig
        /// </summary>
        private void InicializaApp()
        {
            if (Inicializado == false)
            {
                GetNovels = Setter.ObtenGetNovel();
                //DescargasViewModel = new DescargasViewModel(GetNovels);
                Inicializado = true;
            }
        }


        #endregion


        #region ViewModels references

        private object currentView;
        private ConfiguracionViewModel configuracionViewModel;
        private BibliotecaViewModel bibliotecaViewModel;
        private NovelViewModel novelViewModel;
        private DescargasViewModel descargasViewModel;

        /// <summary>
        /// La vista que la app está mostrando actualmente.
        /// </summary>
        public object CurrentView
        {
            get => currentView;
            set => OnPropertyChanged(ref currentView, value);
        }


        /// <summary>
        /// Referencia a una vista de configuracion.
        /// </summary>
        public ConfiguracionViewModel ConfiguracionViewModel
        {
            get => configuracionViewModel;
            set => OnPropertyChanged(ref configuracionViewModel, value);
        }


        /// <summary>
        /// Referencia a una vista de biblioteca.
        /// </summary>
        public BibliotecaViewModel BibliotecaViewModel
        {
            get => bibliotecaViewModel;
            set => OnPropertyChanged(ref bibliotecaViewModel, value);
        }


        public NovelViewModel NovelViewModel
        {
            get => novelViewModel;
            set => OnPropertyChanged(ref novelViewModel, value);
        }

        public DescargasViewModel DescargasViewModel { get => descargasViewModel; set => OnPropertyChanged(ref descargasViewModel, value); }


        #endregion


        #region Comandos de menu barra

        #region Ve Biblioteca
        public RelayCommand Command_VeBiblioteca { get; set; }


        public void VeBiblioteca()
        {
            if(BibliotecaViewModel == null) BibliotecaViewModel = new BibliotecaViewModel();
            CurrentView = BibliotecaViewModel;
        }

        public bool Puedo_VeBibliteca()
        {
            return true;
        }

        #endregion

        #region Ve Configuracion

        public RelayCommand Command_VeConfiguracion { get; set; }

        public void VeConfiguracion()
        {
            if (ConfiguracionViewModel == null) ConfiguracionViewModel = new ConfiguracionViewModel();
            CurrentView = ConfiguracionViewModel;
        } 

        #endregion

        public RelayCommand Command_VeDescargas { get; set; }

        
        public void VeDescargas()
        {
            if(DescargasViewModel == null) DescargasViewModel = new DescargasViewModel(GetNovels);
            CurrentView = DescargasViewModel;
        }


        #endregion
    }
}
