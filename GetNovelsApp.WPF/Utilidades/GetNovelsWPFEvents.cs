using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using GetNovelsApp.WPF.ViewModels;
using Testing;

namespace GetNovelsApp.WPF.Utilidades
{
    public static class GetNovelsWPFEvents
    {
        #region Cambia view model
        public static event Action<object> CambiaViewModel;


        public static void Invoke_Cambia(object ViewModel)
        {
            CambiaViewModel?.Invoke(ViewModel);
        }
        #endregion

        #region AgregaDescarga

        public static event Action<Descarga> AgregaDescarga;

        public static void Invoke_AgregaDescarga(Descarga descarga)
        {
            AgregaDescarga?.Invoke(descarga);
        }

        #endregion
    }
}
