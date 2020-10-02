using System;
using System.Diagnostics;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.WPF.Models
{
    public class ComunicadorWPF : IComunicador
    {
        public ComunicadorWPF()
        {

        }


        IReportero reporteroActual;

        private void EscribeReportero(IReportero reportero)
        {
            if (reporteroActual != reportero)
            {
                Console.WriteLine($"\n-----------> {reportero.Nombre} says: <-----------\n");
                reporteroActual = reportero;
            }
        }
        public string PideInput(string enunciado, IReportero reportero)
        {
            throw new NotImplementedException();
        }

        public void Reporta(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Debug.WriteLine("Reporte: " +enunciado);
        }

        public void ReportaCambioEstado(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Debug.WriteLine("Cambio estado: " + enunciado);
        }

        public void ReportaError(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Debug.WriteLine("Error: " + enunciado);
        }

        public void ReportaErrorMayor(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Debug.WriteLine("Big Error: " + enunciado);
        }

        public void ReportaEspecial(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Debug.WriteLine("Especial: " + enunciado);
        }

        public void ReportaExito(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Debug.WriteLine("Exito: " + enunciado);
        }
    }
}
