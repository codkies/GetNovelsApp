﻿using System;
using GetNovelsApp.Core.Modelos;
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

        public static event Action<INovela> DescargaNovela;

        public static void Invoke_DescargaNovela(INovela novela)
        {
            DescargaNovela?.Invoke(novela);
        }

        #endregion
    }
}
