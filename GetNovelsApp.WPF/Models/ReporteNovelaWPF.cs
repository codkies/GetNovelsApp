using System;
using System.ComponentModel;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.Models
{   
    public class ReporteNovelaWPF : ObservableObject, IReporteNovela
    {
        private INovela objetoReportado;
        private int total;
        private int actual;
        private string mensaje;
        private int identificador;
        private IReportero reportero;

        public ReporteNovelaWPF(INovela novelaDescargada)
        {
            Identificador = novelaDescargada.ID;
            ObjetoReportado = novelaDescargada;
            Actual = novelaDescargada.CapitulosDescargados.Count;
            Total = novelaDescargada.Capitulos.Count;
        }

        public ReporteNovelaWPF(int capitulosTotales, int capitulosDescargados, int novelaID, IReportero reportero, string mensaje = "", INovela novelaDescargada = null)
        {
            Total = capitulosTotales;
            Actual = capitulosDescargados;
            Identificador = novelaID;
            Reportero = reportero;

            //Opcionales:
            ObjetoReportado = novelaDescargada;
            Mensaje = mensaje.Equals("") ? null : mensaje;
        }


        /// <summary>
        /// Puede ser nulo. Cuidado.
        /// </summary>
        public INovela ObjetoReportado { get => objetoReportado; private set => OnPropertyChanged(ref objetoReportado, value); }

        public int PorcentajeDeCompletado => Total > 0 ? (Actual * 100) / Total : 0;

        public int Total { get => total; private set => OnPropertyChanged(ref total, value); }

        public int Actual { get => actual; private set => OnPropertyChanged(ref actual, value); }

        public string Mensaje { get => mensaje; private set => OnPropertyChanged(ref mensaje, value); }

        public int Identificador { get => identificador; private set => OnPropertyChanged(ref identificador, value); }

        public IReportero Reportero { get => reportero; private set => OnPropertyChanged(ref reportero, value); }


        public bool ActualizaReporte(IReporteNovela actualizacion)
        {
            if(identificador != actualizacion.Identificador)
            {
                return false;
            }

            Total = actualizacion.Total;
            Actual = actualizacion.Actual;            
            Reportero = actualizacion.Reportero;

            //Opcionales:
            if(actualizacion.Mensaje != null) Mensaje = actualizacion.Mensaje;
            return true;
        }
      
    }
}
