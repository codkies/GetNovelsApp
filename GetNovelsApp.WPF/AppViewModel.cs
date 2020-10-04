using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private readonly Archivador ar;

        #region Setup

        public AppViewModel()
        {
            GetNovelsWPFEvents.CambiaViewModel += GetNovelsWPFEvents_CambiaViewModel;
            ar = new Archivador();
            Command_VeBiblioteca = new RelayCommand(VeBiblioteca, Puedo_VeBibliteca);
            Command_VeConfiguracion = new RelayCommand(VeConfiguracion);

            InicializaApp();
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
            GetNovels = Setter.ObtenGetNovel();
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


        #region Comandos de menu barra

        public RelayCommand Command_VeBiblioteca { get; set; }

        /// <summary>
        /// Define si el Async para obtener las novelas a mostrar en la biblioteca está corriendo.
        /// </summary>
        bool isExecuting = false;

        public async void VeBiblioteca()
        {
            Debug.WriteLine("Ejecutando");
            isExecuting = true;
            var novelasEnDB = await ar.ObtenTodasNovelasAsync();

            List<NovelaWPF> Novelas = new List<NovelaWPF>();
            foreach (INovela novela in novelasEnDB)
            {
                Novelas.Add((NovelaWPF)novela);
            }

            BibliotecaViewModel = new BibliotecaViewModel(Novelas);
            CurrentView = BibliotecaViewModel;
            isExecuting = false;
            Debug.WriteLine("Finalizado");
        }

        public bool Puedo_VeBibliteca()
        {
            return isExecuting == false;
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
