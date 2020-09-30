using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;
using GetNovelsApp.WPF.ViewModels;

namespace GetNovelsApp.WPF
{
    public class AppViewModel : ObservableObject
    {
        #region Setup

        public AppViewModel()
        {
            InicializaApp();
            InicializaViewModels();
        }

        /// <summary>
        /// Toma referencias de GetNovels y GetNovelsConfig
        /// </summary>
        private void InicializaApp()
        {
            GetNovels = Setter.ObtenGetNovel();
        }


        /// <summary>
        /// Crea instancias de todos los view models y hace que la current view sea la biblioteca.
        /// </summary>
        private void InicializaViewModels()
        {
            BibliotecaViewModel = new BibliotecaViewModel();
            ConfiguracionViewModel = new ConfiguracionViewModel();

            Archivador ar = new Archivador();
            INovela n = ar.BuscaNovelaEnDB(new System.Uri("https://wuxiaworld.site/novel/otherworldly-evil-monarch/"));
            NovelaWPF novela = (NovelaWPF)n;
            NovelViewModel = new NovelViewModel(novela);

            CurrentView = NovelViewModel;
        }

        #endregion

        #region ViewModels references

        public GetNovels GetNovels { get; private set; }

        private object currentView;
        private ConfiguracionViewModel configuracionViewModel;
        private BibliotecaViewModel bibliotecaViewModel;
        private NovelViewModel novelViewModel;

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


        #endregion
    }
}
