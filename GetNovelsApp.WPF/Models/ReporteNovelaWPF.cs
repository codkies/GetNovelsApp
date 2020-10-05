using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.Models
{
    public class ReporteNovelaWPF : ObservableObject, IReporteNovela<NovelaWPF>
    {
        private NovelaWPF objetoDescargado;
        private int total;
        private int actual;
        private string mensaje;
        private int identificador;
        private IReportero reportero;

        public ReporteNovelaWPF(INovela novelaDescargada)
        {
            Identificador = novelaDescargada.ID;
            ObjetoReportado = novelaDescargada as NovelaWPF;
        }

        public ReporteNovelaWPF(int capitulosTotales, int capitulosDescargados, int novelaID, IReportero reportero, string mensaje = "", NovelaWPF novelaDescargada = null)
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
        public NovelaWPF ObjetoReportado { get => objetoDescargado; private set => OnPropertyChanged(ref objetoDescargado, value); }

        public int PorcentajeDeCompletado => Total > 0 ? (Actual * 100) / Total : 0;

        public int Total { get => total; private set => OnPropertyChanged(ref total, value); }

        public int Actual { get => actual; private set => OnPropertyChanged(ref actual, value); }

        public string Mensaje { get => mensaje; private set => OnPropertyChanged(ref mensaje, value); }

        public int Identificador { get => identificador; private set => OnPropertyChanged(ref identificador, value); }

        public IReportero Reportero { get => reportero; private set => OnPropertyChanged(ref reportero, value); }


        public bool ActualizaReporte(IReporte<NovelaWPF> actualizacion)
        {
            if(identificador != actualizacion.Identificador)
            {
                return false;
            }

            Total = actualizacion.Total;
            Actual = actualizacion.Actual;            
            Reportero = actualizacion.Reportero;

            //Opcionales:
            Mensaje = actualizacion.Mensaje.Equals("") ? null : actualizacion.Mensaje;
            return true;
        }
    }
}
