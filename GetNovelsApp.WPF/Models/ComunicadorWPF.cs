using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.WPF.Models
{
    internal class ComunicadorWPF : IComunicador
    {
        public string PideInput(string enunciado, IReportero reportero)
        {
            throw new System.NotImplementedException();
        }

        public void Reporta(string enunciado, IReportero reportero)
        {
        }

        public void ReportaCambioEstado(string enunciado, IReportero reportero)
        {
        }

        public void ReportaError(string enunciado, IReportero reportero)
        {
        }

        public void ReportaErrorMayor(string enunciado, IReportero reportero)
        {
        }

        public void ReportaEspecial(string enunciado, IReportero reportero)
        {
        }

        public void ReportaExito(string enunciado, IReportero reportero)
        {
        }
    }
}