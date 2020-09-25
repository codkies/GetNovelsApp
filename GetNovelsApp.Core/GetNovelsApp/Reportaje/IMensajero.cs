using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GetNovelsApp.Core.Reportaje
{
    public interface IMensajero
    {
        /// <summary>
        /// Toma un input del usuario.
        /// </summary>
        public string PideInput(string enunciado, IReportero reportero);

        /// <summary>
        /// Reporta un error que rompe la aplicacion.
        /// </summary>
        /// <param name="enunciado"></param>
        public void ReportaErrorMayor(string enunciado, IReportero reportero);

        /// <summary>
        /// Reporta un error en el funcionamiento.
        /// </summary>
        /// <param name="enunciado"></param>
        public void ReportaError(string enunciado, IReportero reportero);


        /// <summary>
        /// Reporta que una configuración cambió. 
        /// </summary>
        /// <param name="enunciado"></param>
        public void ReportaCambioEstado(string enunciado, IReportero reportero);


        /// <summary>
        /// Reporta algo.
        /// </summary>
        /// <param name="enunciado"></param>
        public void Reporta(string enunciado, IReportero reportero);



        /// <summary>
        /// Reporta el final de un proceso importante.
        /// </summary>
        /// <param name="enunciado"></param>
        public void ReportaExito(string enunciado, IReportero reportero);


        /// <summary>
        /// Reporta un mensaje especial.
        /// </summary>
        /// <param name="enunciado"></param>
        public void ReportaEspecial(string enunciado, IReportero reportero);

    }
}
