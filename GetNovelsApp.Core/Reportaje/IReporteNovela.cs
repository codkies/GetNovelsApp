using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.Reportaje
{
    /// <summary>
    /// Inherits from IReporte. Usa INovela(s)
    /// </summary>
    public interface IReporteNovela: IReporte<INovela> 
    {
       
    }
}
