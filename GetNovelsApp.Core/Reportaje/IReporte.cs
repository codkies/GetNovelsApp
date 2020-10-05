using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.Reportaje
{
    /// <summary>
    /// Interfaz para los mensajes dentro del 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReporte<T>
    {
        public int PorcentajeDeCompletado { get; }

        public T ObjetoReportado { get; }

        public int Total { get; }

        public int Actual { get; }

        public string Mensaje { get; }

        public int Identificador { get; }

        public IReportero Reportero { get; }
    }

    public interface IReporteNovela<T> : IReporte<T> where T : INovela
    {

    }

    public class ReporteNovela
    {

    }

}
