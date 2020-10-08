namespace GetNovelsApp.Core.Reportaje
{
    public class Reporte : IReporte
    {
        private string nombreItem;
        private int total;
        private int actual;
        private string mensaje;
        private int identificador;
        private IReportero reportero;

        public Reporte(int total, int actual, string estado, int identificador, IReportero reportero, string nombreItem)
        {
            Total = total;
            Actual = actual;
            Estado = estado;
            Identificador = identificador;
            Reportero = reportero;
            NombreItem = nombreItem;
        }

        public int PorcentajeDeCompletado => (Actual * 100) / Total;

        public int Total { get => total; private set => total = value; }

        public int Actual { get => actual; private set => actual = value; }

        public string Estado { get => mensaje; private set => mensaje = value; }

        public int Identificador { get => identificador; private set => identificador = value; }

        public IReportero Reportero { get => reportero; set => reportero = value; }

        public string NombreItem { get => nombreItem; set => nombreItem = value; }
    }



}
