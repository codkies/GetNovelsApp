using System.Diagnostics.Tracing;
using GetNovelsApp.Core.Modelos;

namespace Testing
{ 
    /// <summary>
    /// This class is kinda dumb...
    /// </summary>
    public class Descarga 
    {
        public Descarga(INovela Novela)
        {
            this.Novela = Novela;
            PorcentajeDescarga = Novela.PorcentajeDescarga;

        }

        public INovela Novela { get; set; }


        public int PorcentajeDescarga { get; set; }
    }

}
