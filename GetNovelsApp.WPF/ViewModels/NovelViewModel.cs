using GetNovelsApp.Core;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class NovelViewModel : ObservableObject, IReportero
    {

        public string Nombre => "NovelViewModel";
        public NovelViewModel(NovelaWPF novela)
        {            
            NovelaEnVista = novela;
            if(novela.ImagenLink != null) ActualizaImagen();

            Command_DescargaNovela = new RelayCommand(DescargaNovelaAsync, Puedo_DescargaNovela);
            Command_Leer = new RelayCommand(Leer, Puede_Leer);
        }


        private void ActualizaImagen()
        {
            PathImagenNovela = EncontradorImagen.DescargaImagen(NovelaEnVista.ImagenLink.ToString());
        }


        #region Props
        private NovelaWPF novelaEnVista;
        private string pathImagenNovela;

        public NovelaWPF NovelaEnVista { get => novelaEnVista; set => OnPropertyChanged(ref novelaEnVista, value); }

        public string PathImagenNovela { get => pathImagenNovela; private set => OnPropertyChanged(ref pathImagenNovela, value); }
        #endregion


        #region Descarga

        public RelayCommand Command_DescargaNovela { get; set; }

        bool descargando = false;

        public void DescargaNovelaAsync()
        {
            GetNovelsWPFEvents.Invoke_DescargaNovela(NovelaEnVista);
            descargando = true;
        }

        public bool Puedo_DescargaNovela()
        {
            return !NovelaEnVista.EstoyCompleta & descargando == false;
        }

        #endregion


        #region Leer

        public RelayCommand Command_Leer { get; set; }


        private void Leer()
        {
            ReporteWPF reporte = (ReporteWPF)GetNovelsFactory.FabricaReporteNovela(NovelaEnVista.CapitulosDescargados.Count, 0, 
                                                                                   "Descargando", this, NovelaEnVista.Titulo);
            ManagerTareas.MuestraReporte(reporte);

            Progreso progreso = new Progreso();
            progreso.ProgressChanged += Progreso_ProgressChanged;
            AppViewModel.GetNovels.MyEmpaquetador.ImprimeNovela(NovelaEnVista, TiposDocumentos.PDF);
        }

       

        private bool Puede_Leer()
        {
            return NovelaEnVista.CantidadCapitulosDescargados > 1;
        }

        #endregion

        #region event handler

        private void Progreso_ProgressChanged(object sender, IReporte e)
        {
            ManagerTareas.ActualizaReporte(e);
        }


        #endregion

    }
}
