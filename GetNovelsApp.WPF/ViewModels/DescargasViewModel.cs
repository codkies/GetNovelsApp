using System;
using System.Collections.ObjectModel;
using System.Linq;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class DescargasViewModel : ObservableObject
    {
        #region Cons fields

        /// <summary>
        /// Referencia a la app.
        /// </summary>
        private readonly GetNovels GetNovels;
        private ObservableCollection<ReporteNovelaWPF> descargas = new ObservableCollection<ReporteNovelaWPF>();

        #endregion


        #region Ctors

        public DescargasViewModel(GetNovels getNovels)
        {
            GetNovels = getNovels;
            GetNovelsWPFEvents.DescargaNovela += GetNovelsWPFEvents_DescargaNovela;
        }


        #endregion


        #region Props 


        public ObservableCollection<ReporteNovelaWPF> Descargas { get => descargas; set => OnPropertyChanged(ref descargas, value); }


        #endregion

        #region Metodos

        private async void GetNovelsWPFEvents_DescargaNovela(INovela novela)
        {
            RecordDescarga(novela);

            ProgresoNovela progresoNovela = new ProgresoNovela();

            progresoNovela.ProgressChanged += ProgresoNovela_ProgressChanged;

            bool exito = await GetNovels.AgregaAlQueue(novela, progresoNovela);

            if (exito == false)
            {
                throw new NotImplementedException($"no se pudo agregar {novela.Titulo} a las descargas.");
            }
        }

        private void RecordDescarga(INovela novela)
        {
            ReporteNovelaWPF reporteVacio = new ReporteNovelaWPF(novela);
            Descargas.Add(reporteVacio);

            Tarea tarea = new Tarea("Descargando", novela.Titulo, novela.PorcentajeDescarga, novela.ID);
            GetNovelsWPFEvents.Invoke_NotificaTarea(tarea);
        }

        private void ProgresoNovela_ProgressChanged(object sender, IReporteNovela e)
        {
            //Encontrando la novela a la que este cambio está notificando.
            var descarga = Descargas.Where(x => x.Identificador == e.Identificador).First();
            if (descarga == null)
            {
                throw new Exception("Se intento actualizar el reporte de una descarga del cual no se tiene referencia\n- DescargasViewModel.");
            }

            int tareaID = e.Identificador;
            int progreso = e.PorcentajeDeCompletado;
            string estado = e.PorcentajeDeCompletado < 100 ? "Descargando" : "Completado";

            GetNovelsWPFEvents.Invoke_ActualizaTarea(tareaID, progreso, estado);

            descarga?.ActualizaReporte(e);
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
