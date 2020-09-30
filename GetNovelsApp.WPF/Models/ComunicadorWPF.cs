using System;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.WPF.Models
{
    public class ComunicadorWPF : IComunicador
    {
        public ComunicadorWPF()
        {

        }


        public string PideInput(string enunciado, IReportero reportero)
        {
            throw new NotImplementedException();
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
