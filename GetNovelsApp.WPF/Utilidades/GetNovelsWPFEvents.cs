using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Modelos;

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

        public static event Action<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>> DescargaNovela;

        public static void Invoke_DescargaNovela(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela)
        {
            DescargaNovela?.Invoke(novela);
        }

        #endregion

        #region Barra de tareas

        public static event Action TareasCambio;

        public static void Invoke_TareasCambio()
        {
            TareasCambio?.Invoke();
        }


        #endregion
    }


}
