using System;
using System.ComponentModel;
using System.Windows.Controls;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.Models
{   
    public class ReporteWPF : ObservableObject, IReporte
    {
        #region Backing fields
        private string nombreItem;
        private int total;
        private int actual;
        private string mensaje;
        private int identificador;
        private IReportero reportero;
        private int porcentajeCompletado;
        #endregion

        #region Ctors
        public ReporteWPF(int total, int actual, string estado, int identificador, IReportero reportero, string nombreItem)
        {
            Total = total;
            Actual = actual;
            Estado = estado;
            Identificador = identificador;
            Reportero = reportero;
            NombreItem = nombreItem;
        }
        #endregion


        #region Props

        public int Total { get => total; set 
            {
                OnPropertyChanged(ref total, value);
                PorcentajeCambio();
            } 
        }

        public int Actual { get => actual; set
            {
                OnPropertyChanged(ref actual, value);
                PorcentajeCambio();
            }
        }

        public string Estado { get => mensaje; set => OnPropertyChanged(ref mensaje, value); }

        public int Identificador { get => identificador; private set => OnPropertyChanged(ref identificador, value); }

        public IReportero Reportero { get => reportero; private set => OnPropertyChanged(ref reportero, value); }

        public string NombreItem { get => nombreItem; private set => OnPropertyChanged(ref nombreItem, value); }

        public int PorcentajeDeCompletado { get => porcentajeCompletado; private set => OnPropertyChanged(ref porcentajeCompletado, value); }

        #endregion


        #region Helpers

        private void PorcentajeCambio()
        {
            PorcentajeDeCompletado = Total > 0 ? (Actual * 100) / Total : 0;
        }

        #endregion

    }
}
