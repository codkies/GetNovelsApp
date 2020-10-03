using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

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

            Command_SalvaCambios = new RelayCommand(SalvaCambios, Puede_SalvaCambios);
        }

        private void ObtenWebsitesSoportados()
        {
            var perfiles = ar.ObtenPerfiles();
            foreach (var perfil in perfiles)
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

        public List<string> WebsitesSoportados { get => websitesSoportados; set => OnPropertyChanged(ref websitesSoportados, value); }

        public string CapitulosPorDocumento { get => capitulosPorDocumento; set => OnPropertyChanged(ref capitulosPorDocumento, value); }

        public string Carpeta { get => carpeta; set => OnPropertyChanged(ref carpeta, value); }

        public string TamañoBatchDescarga { get => tamañoBatchDescarga; set => OnPropertyChanged(ref tamañoBatchDescarga, value); } 
        #endregion


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
                   int.TryParse(CapitulosPorDocumento, out _);
        }
    }
}
