using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;
using GetNovelsApp.WPF.ViewModels;
using GetNovelsApp.WPF.Views;

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

            GetNovelsWPFEvents.CambiaViewModel += GetNovelsWPFEvents_CambiaViewModel;

            Command_VeBiblioteca = new RelayCommand(VeBiblioteca, Puedo_VeBibliteca);
            Command_VeConfiguracion = new RelayCommand(VeConfiguracion);

            VeBiblioteca();
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
            if(Inicializado == false)
            {
                GetNovels = Setter.ObtenGetNovel();
                Inicializado = true;
            }
        }


        #endregion


        #region ViewModels references

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


        #region Comandos de menu barra

        public RelayCommand Command_VeBiblioteca { get; set; }

        
        public void VeBiblioteca()
        {            
            BibliotecaViewModel = new BibliotecaViewModel();
            CurrentView = BibliotecaViewModel;
        }

        public bool Puedo_VeBibliteca()
        {
            return true;
        }

        
        public RelayCommand Command_VeConfiguracion { get; set; }

        public void VeConfiguracion()
        {
            ConfiguracionViewModel = new ConfiguracionViewModel();
            CurrentView = ConfiguracionViewModel;
        }




        #endregion
    }
}
