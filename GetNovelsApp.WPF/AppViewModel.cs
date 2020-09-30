using GetNovelsApp.Core.Conexiones.DB;
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
            InicializaViewModels();
        }


        /// <summary>
        /// Crea instancias de todos los view models y hace que la current view sea la biblioteca.
        /// </summary>
        private void InicializaViewModels()
        {
            bibliotecaViewModel = new BibliotecaViewModel();
            configuracionViewModel = new ConfiguracionViewModel();
            CurrentView = bibliotecaViewModel;
        }

        #endregion

        #region ViewModels references

        private object currentView;
        private ConfiguracionViewModel configuracionViewModel;
        private BibliotecaViewModel bibliotecaViewModel;

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

        #endregion
    }
}
