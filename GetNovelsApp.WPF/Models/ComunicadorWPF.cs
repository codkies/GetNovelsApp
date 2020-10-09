using System.Threading;
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
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Reporte\n {enunciado}");
        }

        public void ReportaCambioEstado(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Cambio de estado\n {enunciado}");
        }

        public void ReportaError(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Error {Thread.CurrentThread.Name}\n {enunciado}");
        }

        public void ReportaErrorMayor(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Error grande {Thread.CurrentThread.Name}\n {enunciado}");
        }

        public void ReportaEspecial(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Especial\n {enunciado}");
        }

        public void ReportaExito(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Exito\n {enunciado}");
        }
    }
}