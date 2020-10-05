namespace GetNovelsApp.Core.Reportaje
{

    /// <summary>
    /// Interfaz para los mensajes dentro del 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IReporte<out T>
    {
        public int PorcentajeDeCompletado { get; }

        public T ObjetoReportado { get; }

        public int Total { get; }

        public int Actual { get; }

        public string Mensaje { get; }

        public int Identificador { get; }

        public IReportero Reportero { get; }
    }
}
