using System.Web.Compilation;

namespace GetNovelsApp.Core.Reportaje
{

    /// <summary>
    /// Interfaz para los mensajes dentro del 
    /// </summary>
    /// <typeparam name="T"></typeparam> 
    public interface IReporte
    {
        public int PorcentajeDeCompletado { get; }

        public string NombreItem { get; }

        public int Total { get; }

        public int Actual { get; }

        public string Estado { get; }

        public int Identificador { get; }

        public IReportero Reportero { get; }
    }



}
