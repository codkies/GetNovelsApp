using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Services;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class DescargasViewModel : ObservableObject , IReportero
    {
        public string Nombre => "DescargasViewModel";

        #region Cons fields

        /// <summary>
        /// Referencia a la app.
        /// </summary>
        private readonly GetNovels GetNovels;
        private ObservableCollection<ReporteWPF> descargas = new ObservableCollection<ReporteWPF>();

        #endregion


        #region Ctors

        public DescargasViewModel(GetNovels getNovels)
        {
            GetNovels = getNovels;
            GetNovelsWPFEvents.DescargaNovela += GetNovelsWPFEvents_DescargaNovela;
        }


        #endregion


        #region Props 


        public ObservableCollection<ReporteWPF> DescargasNovelas { get => descargas; set => OnPropertyChanged(ref descargas, value); }


        #endregion


        #region Metodos

        private async void GetNovelsWPFEvents_DescargaNovela(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela)
        {
            ReporteWPF reporte = (ReporteWPF)GetNovelsFactory.FabricaReporteNovela(novela.LinksDeCapitulos.ToList().Count, novela.CapitulosPorDescargar.ToList().Count, "Descargando", this, novela.Titulo);

            ManagerTareas.MuestraReporte(reporte);
            
            DescargasNovelas.Add(reporte);

            Progreso progresoNovela = new Progreso();

            progresoNovela.ProgressChanged += ProgresoNovela_ProgressChanged;

            bool exito = await GetNovels.AgregaAlQueue(novela, progresoNovela);

            if (exito == false)
            {
                throw new NotImplementedException($"no se pudo agregar {novela.Titulo} a las descargas.");
            }
        }

        private void ProgresoNovela_ProgressChanged(object sender, IReporte e)
        {
            string estado = e.PorcentajeDeCompletado < 100 ? "Descargando" : "Completado";

            foreach (ReporteWPF reporte in DescargasNovelas)
            {
                if (reporte.NombreItem.Equals(e.NombreItem))
                {
                    reporte.Total = e.Total;
                    reporte.Actual = e.Actual;
                    reporte.Estado = e.Estado;
                    break;
                }
            }
            ManagerTareas.ActualizaReporte(e);            
        }      


        #endregion



        #region Commands

        public void PausaDescargas()
        {

        }

        public void ComienzaDescargas()
        {

        }

        #endregion

    }
}
