using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.Reportaje;

namespace GetAppsNovel.ConsoleVersion
{
    public class ConsoleUI : IMensajero
    {
        #region Colores

        private const ConsoleColor ColorError = ConsoleColor.Red;
        private const ConsoleColor ColorReportero = ConsoleColor.Magenta;
        private const ConsoleColor ColorFormulario = ConsoleColor.Cyan;

        private const ConsoleColor ColorNotificacion = ConsoleColor.DarkYellow;
        private const ConsoleColor ColorCambioEstado = ConsoleColor.Yellow;

        private const ConsoleColor ColorEspecial = ConsoleColor.White;
        private const ConsoleColor ColorExito = ConsoleColor.DarkCyan;


        #endregion

        IReportero reporteroActual = null;

        private void EscribeReportero(IReportero reportero)
        {
            Console.ForegroundColor = ColorReportero;
            if(reporteroActual != reportero)
            {
                Console.WriteLine($"\n-----------> {reportero.Nombre} says: <-----------\n");
                reporteroActual = reportero;
            }            
        }


        public string PideInput(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorFormulario;
            EscribeEnunciado(enunciado);
            Console.ForegroundColor = ColorEspecial;
            return Console.ReadLine();
        }

        private static void EscribeEnunciado(string enunciado)
        {
            Console.WriteLine(enunciado);
        }

        public void Reporta(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorNotificacion;
            EscribeEnunciado(enunciado);
        }

        public void ReportaCambioEstado(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorCambioEstado;
            EscribeEnunciado(enunciado);
        }

        public void ReportaError(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorError;
            EscribeEnunciado(enunciado);
        }

        public void ReportaErrorMayor(string enunciado, IReportero reportero)
        {            
            enunciado += " Presiona enter para cerrar el programa.";
            ReportaError(enunciado, reportero);
            Console.ReadLine();
            Environment.Exit(0);
        }

        public void ReportaEspecial(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorEspecial;
            EscribeEnunciado(enunciado);
        }

        public void ReportaExito(string enunciado, IReportero reportero)
        {
            EscribeReportero(reportero);
            Console.ForegroundColor = ColorExito;
            EscribeEnunciado(enunciado);
        }
    }
}
