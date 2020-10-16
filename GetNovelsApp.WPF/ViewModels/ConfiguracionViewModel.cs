﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.WPF.Utilidades;
using GetNovelsApp.WPF.Views;

namespace GetNovelsApp.WPF.ViewModels
{
    public class ConfiguracionViewModel : ObservableObject
    {
        #region const fields

        private readonly Archivador ar;
        private IConfig configDefault;

        #endregion


        #region Setup
        public ConfiguracionViewModel()
        {
            ar = new Archivador();
            ObtenWebsitesSoportados();

            configDefault = ar.ObtenConfiguracion();

            Carpeta = configDefault.FolderPath;
            CapitulosPorDocumento = configDefault.CapitulosPorDocumento.ToString();
            TamañoBatchDescarga = configDefault.TamañoBatch.ToString();

            //Botones
            Command_SalvaCambios = new RelayCommand(SalvaCambios, Puede_SalvaCambios);
            Command_AggPerfil = new RelayCommand(AggPerfil, Puede_AggPerfil);
            Command_EditPerfil = new RelayCommand<string>(EditPerfil);
            Command_AbrirPath = new RelayCommand(AbrirPath, Puede_AbrirPath);
            //Events
            GetNovelsEvents.WebsitesCambiaron += ObtenWebsitesSoportados;

        }

        private void ObtenWebsitesSoportados()
        {
            WebsitesSoportados = new List<string>();
            Perfiles = ar.ObtenPerfiles();
            foreach (var perfil in Perfiles)
            {
                WebsitesSoportados.Add(perfil.Dominio);
            }
        }

        #endregion


        #region Props
        private List<string> websitesSoportados = new List<string>();
        private string capitulosPorDocumento;
        private string carpeta;
        private string tamañoBatchDescarga;

        private List<IPath> Perfiles = new List<IPath>();

        public List<string> WebsitesSoportados { get => websitesSoportados; set => OnPropertyChanged(ref websitesSoportados, value); }

        public string CapitulosPorDocumento { get => capitulosPorDocumento; set => OnPropertyChanged(ref capitulosPorDocumento, value); }

        public string Carpeta { get => carpeta; set => OnPropertyChanged(ref carpeta, value); }

        public string TamañoBatchDescarga { get => tamañoBatchDescarga; set => OnPropertyChanged(ref tamañoBatchDescarga, value); }
        #endregion



        #region Salvar cambios
        public RelayCommand Command_SalvaCambios { get; set; }


        public void SalvaCambios()
        {
            var confignueva = GetNovelsFactory.FabricaConfiguracion(Carpeta, int.Parse(TamañoBatchDescarga), int.Parse(CapitulosPorDocumento));

            ar.ActualizaConfiguracion(confignueva);
            configDefault = ar.ObtenConfiguracion();

        }

        public bool Puede_SalvaCambios()
        {
            return TamañoBatchDescarga != null &
                   Carpeta != null &
                   CapitulosPorDocumento != null &
                   int.TryParse(TamañoBatchDescarga, out _) &
                   int.TryParse(CapitulosPorDocumento, out _) &
                   HayCambios;
        } 

        /// <summary>
        /// El usuario cambió algo con respecto a la configuración inicial.
        /// </summary>
        private bool HayCambios 
        { 
            get
            {
                bool cambioCarpeta = !Carpeta.Equals(configDefault.FolderPath);

                int.TryParse(CapitulosPorDocumento, out int caps);
                bool cambioCaps = configDefault.CapitulosPorDocumento != caps;

                int.TryParse(TamañoBatchDescarga, out int batch);
                bool cambioBatch = configDefault.TamañoBatch != batch;

                return cambioBatch | cambioCarpeta | cambioCaps;
            } 
        }

        #endregion


        #region Comando Agrega Perfil
        public RelayCommand Command_AggPerfil { get; set; }

        public void AggPerfil()
        {
            AddWebsiteView websiteView = new AddWebsiteView();
            websiteView.Show();
        }

        public bool Puede_AggPerfil()
        {
            return true;
        }
        #endregion


        #region Comando Edita Perfil

        public RelayCommand<string> Command_EditPerfil { get; set; }

        public void EditPerfil(string dominio)
        {            
            IPath Perfil;
            try
            {
                Perfil = Perfiles.Where(x => x.Dominio.Equals(dominio)).First();
                EditPefilViewModel editPefilViewModel = new EditPefilViewModel(Perfil);
                GetNovelsWPFEvents.Invoke_Cambia(editPefilViewModel);
            }
            catch (Exception ex)
            {
                throw new Exception($"Exception intentando editar un perfil:\n {ex.Message}");
            }
            finally
            {
                
            }

        }

        #endregion


        #region Comando Abrir carpeta

        public RelayCommand Command_AbrirPath { get; set; }

        private void AbrirPath()
        {
            try
            {
                Process.Start($@"{Carpeta}");
            }
            catch (Win32Exception win32Exception)
            {
                //The system cannot find the file specified...
                Debug.WriteLine(win32Exception.Message);
            }
        }

        private bool Puede_AbrirPath()
        {
            return string.IsNullOrEmpty(Carpeta) == false;
        }

        #endregion
    }
}
