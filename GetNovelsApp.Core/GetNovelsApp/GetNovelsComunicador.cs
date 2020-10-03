using System;
using System.Runtime.CompilerServices;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.Core
{
    /// <summary>
    /// Encargado de enviar mensajes al usuario
    /// </summary>
    public static class GetNovelsComunicador
    {
        public static void InicializaComunicador(IComunicador mensajero)
        {
            Mensajero = mensajero;
        }

        private static IComunicador Mensajero;



        #region Implementaciones del IComunicador


        /// <summary>
        /// Toma un input del usuario.
        /// </summary>
        public static string PideInput(string enunciado, IReportero reportero)
        {
            return Mensajero.PideInput(enunciado, reportero);
        }


        /// <summary>
        /// Reporta un error que rompe la aplicacion.
        /// </summary>
        public static void ReportaErrorMayor(string enunciado, IReportero reportero)
        {
            Mensajero.ReportaErrorMayor(enunciado, reportero);
        }


        /// <summary>
        /// Reporta que una configuración cambió. 
        /// </summary>
        public static void ReportaCambioEstado(string enunciado, IReportero reportero)
        {
            Mensajero.ReportaCambioEstado(enunciado, reportero);
        }


        /// <summary>
        /// Reporta un error en el funcionamiento.
        /// </summary>
        public static void ReportaError(string enunciado, IReportero reportero)
        {
            Mensajero.ReportaError(enunciado, reportero);
        }


        /// <summary>
        /// Reporta algo.
        /// </summary>
        public static void Reporta(string enunciado, IReportero reportero)
        {
            Mensajero.Reporta(enunciado, reportero);
        }


        /// <summary>
        /// Reporta el final de un proceso importante.
        /// </summary>
        public static void ReportaExito(string enunciado, IReportero reportero)
        {
            Mensajero.ReportaExito(enunciado, reportero);
        }


        /// <summary>
        /// Reporta un mensaje especial.
        /// </summary>
        public static void ReportaEspecial(string enunciado, IReportero reportero)
        {
            Mensajero.ReportaEspecial(enunciado, reportero);
        }

        #endregion
    }
}