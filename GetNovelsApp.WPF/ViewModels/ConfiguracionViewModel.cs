using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class ConfiguracionViewModel : ObservableObject
    {
        public ConfiguracionViewModel(List<string> WebsiteSoportados)
        {
            this.WebsiteSoportados = WebsiteSoportados;
        }

        public List<string> WebsiteSoportados { get; set; }


        IConfig configuracion;

        public void ActualizaConfiguraciones()
        {
            /*
             Factory.CreaConfiguracion(Parametros de esta clase);
             */

            
        }
    }
}
