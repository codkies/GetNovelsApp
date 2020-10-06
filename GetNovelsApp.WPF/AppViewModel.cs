using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Resources;
using System.Runtime;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
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

            //comandos and events:
            GetNovelsWPFEvents.CambiaViewModel += GetNovelsWPFEvents_CambiaViewModel;
            GetNovelsWPFEvents.NotificaTarea += AgregaTarea;
            GetNovelsWPFEvents.ActualizaTarea += ActualizaTarea;


            Command_VeBiblioteca = new RelayCommand(VeBiblioteca, Puedo_VeBibliteca);
            Command_VeConfiguracion = new RelayCommand(VeConfiguracion);
            Command_VeDescargas = new RelayCommand(VeDescargas);

            //VeDescargas();
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
            if (Inicializado) return;

            GetNovels = Setter.ObtenGetNovel();
            BibliotecaViewModel = new BibliotecaViewModel();
            ConfiguracionViewModel = new ConfiguracionViewModel();
            DescargasViewModel = new DescargasViewModel(GetNovels);
            Inicializado = true;
        }


        #endregion


        #region ViewModels references

        private object currentView;
        private ConfiguracionViewModel configuracionViewModel;
        private BibliotecaViewModel bibliotecaViewModel;
        private NovelViewModel novelViewModel;
        private DescargasViewModel descargasViewModel;
        private string mensajeDeEstado;

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
            if (BibliotecaViewModel == null) BibliotecaViewModel = new BibliotecaViewModel();
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
            if (DescargasViewModel == null) DescargasViewModel = new DescargasViewModel(GetNovels);
            CurrentView = DescargasViewModel;
        }


        #endregion


        #region Propieades de barra de tareas

        public ObservableCollection<ITarea> Tareas { get; set; }


        public string MensajeDeEstado { get => mensajeDeEstado; set => OnPropertyChanged(ref mensajeDeEstado, value); }

        public Tarea TareaCorriendo 
        {
            get
            {
                return Tareas?.Where(t => t.PorcentajeTarea < 100).First() as Tarea;
            }
        }


        #region shorthands para el mensaje de estado
        private string IndexTareaCorriendo
        {
            get
            {
                if (TareaCorriendo == null) return string.Empty;                
                return (Tareas.IndexOf(TareaCorriendo) + 1).ToString();
                //nada de (0/1)
            }
        }

        private string EstadoTareas => TareaCorriendo == null ? string.Empty : TareaCorriendo.Estado;

        private string Item => TareaCorriendo == null ? string.Empty : TareaCorriendo.Item;

        private string PorcentajeTareaCorriendo => TareaCorriendo == null ? string.Empty : TareaCorriendo.PorcentajeTarea.ToString();

        private string TotalTareas => Tareas == null ? string.Empty : Tareas.Count.ToString(); 
        #endregion

        private void ActualizaMensajeDeEstado()
        {
            MensajeDeEstado = $"{EstadoTareas} '{Item}' ({PorcentajeTareaCorriendo}%)... ({IndexTareaCorriendo}/{TotalTareas})";
        }

        private void AgregaTarea(ITarea tarea)
        {
            if (Tareas == null) Tareas = new ObservableCollection<ITarea>();
            if (Tareas.Contains(tarea) == false)
            {
                Tareas.Add(tarea);
                ActualizaMensajeDeEstado();
            }
            else
            {
                throw new NotImplementedException("Se intentó agregar una tarea que ya existe");
            }
        }

        private void ActualizaTarea(int tareaID, int progreso, string estado)
        {
            if (Tareas == null)
            {
                throw new NotImplementedException("Se intentó actualizar una tarea cuando nunca se han creado tareas...");
            }
            ITarea tareaEnRecord = Tareas.Where(x => x.ID == tareaID).First();

            if (tareaEnRecord == null)
            {
                throw new NotImplementedException("Se encontró la tarea que se intentó actualizar.");
            }

            tareaEnRecord.ActualizaProgreso(tareaID, progreso, estado);
            ActualizaMensajeDeEstado();
        }

        #endregion
    }
}
