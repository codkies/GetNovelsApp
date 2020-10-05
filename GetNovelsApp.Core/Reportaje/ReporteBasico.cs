using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.Reportaje
{
    public class ReporteBasico : IReporte<INovela>
    {
        private INovela objetoDescargado;
        private int total;
        private int actual;
        private string mensaje;
        private int identificador;
        private IReportero reportero;

        public ReporteBasico(int capitulosTotales, int capitulosDescargados, int novelaID, IReportero reportero, string mensaje = "", INovela objetoDescargado = null)
        {
            Total = capitulosTotales;
            Actual = capitulosDescargados;
            Identificador = novelaID;
            Reportero = reportero;

            //Opcionales:
            ObjetoReportado = objetoDescargado;
            Mensaje = mensaje.Equals("") ? null : mensaje;
        }

        /// <summary>
        /// Puede ser nulo. Cuidado.
        /// </summary>
        public INovela ObjetoReportado { get => objetoDescargado; private set => objetoDescargado = value; }

        public int PorcentajeDeCompletado => (Actual * 100) / Total;

        public int Total { get => total; private set => total = value; }

        public int Actual { get => actual; private set => actual = value; }

        public string Mensaje { get => mensaje; private set => mensaje = value; }

        public int Identificador { get => identificador; private set => identificador = value; }

        public IReportero Reportero { get => reportero; set => reportero = value; }
    }


}
