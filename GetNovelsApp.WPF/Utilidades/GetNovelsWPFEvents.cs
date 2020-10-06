using System;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.WPF.Models;

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

        public static event Action<INovela> DescargaNovela;

        public static void Invoke_DescargaNovela(INovela novela)
        {
            DescargaNovela?.Invoke(novela);
        }

        #endregion

        #region Comunica una tarea

        public static event Action<ITarea> NotificaTarea;

        public static void Invoke_NotificaTarea(ITarea tarea)
        {
            NotificaTarea?.Invoke(tarea);
        }


        public static event Action<int, int, string> ActualizaTarea;

        public static void Invoke_ActualizaTarea(int tareaID, int progreso, string estado)
        {
            ActualizaTarea?.Invoke(tareaID, progreso, estado);
        }


        #endregion
    }


}
