using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;
using Testing;

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
            ReporteNovelaWPF reporteVacio = new ReporteNovelaWPF(novela);
            Descargas.Add(reporteVacio);

            ProgresoNovela progresoNovela = new ProgresoNovela();

            progresoNovela.ProgressChanged += ProgresoNovela_ProgressChanged;

            bool exito = await GetNovels.AgregaAlQueue(novela, progresoNovela);

            if(exito == false)
            {
                throw new NotImplementedException($"no se pudo agregar {novela.Titulo} a las descargas.");
            }
        }

        private void ProgresoNovela_ProgressChanged(object sender, IReporteNovela e)
        {
            //Encontrando la novela a la que este cambio está notificando.
            var descarga = Descargas.Where(x => x.Identificador == e.Identificador).First();
            descarga?.ActualizaReporte(e);
            if (descarga == null)
            {
                throw new Exception("Se intento actualizar el reporte de una descarga del cual no se tiene referencia\n- DescargasViewModel.");
            }
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
