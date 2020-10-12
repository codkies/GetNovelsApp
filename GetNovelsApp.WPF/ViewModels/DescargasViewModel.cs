using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class DescargasViewModel : ObservableObject , IReportero
    {

        #region Cons fields

        /// <summary>
        /// Referencia a la app.
        /// </summary>
        private readonly GetNovels GetNovels;
        private ObservableCollection<ReporteWPF> descargas = new ObservableCollection<ReporteWPF>();
        private readonly Archivador ar;       

        private int CantidadNovelasIncompletas = 0;
        public string Nombre => "DescargasViewModel";

        /// <summary>
        /// Estado del script. Returns true si una novela se está descargando.
        /// </summary>
        bool descargando = false;

        #endregion


        #region Ctors 

        public DescargasViewModel(GetNovels getNovels)
        {
            GetNovels = getNovels;
            ar = new Archivador();
            GetNovelsWPFEvents.DescargaNovela += GetNovelsWPFEvents_DescargaNovela;
            GetNovelsEvents.NovelaAgregadaADB += GetNovelsEvents_NovelaAgregadaADB;
            Command_DescargarBib = new RelayCommand(DescargarBib, Puede_DescargarBib);
            GetNovelsEvents_NovelaAgregadaADB(); //run it once at the beginning.
        }


        #endregion


        #region Props 


        public ObservableCollection<ReporteWPF> DescargasNovelas { get => descargas; set => OnPropertyChanged(ref descargas, value); }

        public Queue<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>> NovelasADescargar 
            = new Queue<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>>();
        #endregion


        #region Core methods

        /// <summary>
        /// Hace todas las preparaciones para agregar una novela a la lista de descargas. Comienza las descargas.
        /// </summary>
        /// <param name="novela"></param>
        private void IngresaNovelaADescargas(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela)
        {
            //Reporta que una descarga fue agregada.
            ReporteWPF reporte = (ReporteWPF)GetNovelsFactory.FabricaReporteNovela(novela.LinksDeCapitulos.ToList().Count, novela.CapitulosDescargados.ToList().Count, "Descargando", this, novela.Titulo);
            ManagerTareas.MuestraReporte(reporte);

            //Esta coleccion es la que está conectada a la view. Se está agregando.
            DescargasNovelas.Add(reporte);

            //Esta coleccion es un Queue de novelas a descargar.
            NovelasADescargar.Enqueue(novela);

            //Descargando es solo true si "Comienza Descargas" ya está corriendo.
            if (descargando == false) ComienzaDescargas();
        }

        private async Task ComienzaDescargas()
        {
            if (descargando) return; //por si acaso de llama una instancia de este metodo mientras otra está corriendo.

            //Mientras haya novelas por descargar, esto no se detendrá.
            while (NovelasADescargar.Any() == true)
            {
                //Preps                
                descargando = true;

                //Referencias a la novela a descargar
                var novela = NovelasADescargar.Dequeue();
                Progreso progresoNovela = new Progreso();
                progresoNovela.ProgressChanged += ProgresoNovela_ProgressChanged;

                //Descargando
                bool exito = await GetNovels.AgregaAlQueue(novela, progresoNovela);
                if (exito == false) throw new Exception("La descarga fallo");

                //Ending
                descargando = false;
            }            
        }


        #endregion



        #region Commands

        public RelayCommand Command_DescargarBib { get; set; }


        /// <summary>
        /// Define si el boton de descarga biblioteca fue presionado o no. Usado para el predicado del comando "DescargaBib".
        /// </summary>
        bool descargandoBiblioteca = false;


        /// <summary>
        /// Lo que ocurre cuando el usuario ejecuta el comando "Command_DescargarBib"
        /// </summary>
        /// <returns></returns>
        private async void DescargarBib()
        {
            descargandoBiblioteca = true;
            Debug.WriteLine("descargando biblioteca...");

            var allNovels = await Task.Run(() => ar.ObtenTodasNovelas());

            var novelasEnBib = allNovels.Where(x => !x.EstoyCompleta).ToList();

            CantidadNovelasIncompletas = novelasEnBib.Count;

            novelasEnBib.ForEach(j => IngresaNovelaADescargas(j));
        }

        /// <summary>
        /// Define si se puede descargar toda la libreria
        /// </summary>
        /// <returns></returns>
        private bool Puede_DescargarBib()
        {
            return descargandoBiblioteca == false & CantidadNovelasIncompletas > 0;
        }


        #endregion



        #region event handlers


        /// <summary>
        /// Cada vez que se agrega una novela nueva a la biblioteca, se actualiza la cantidad de novelas incompletas.
        /// </summary>
        private void GetNovelsEvents_NovelaAgregadaADB()
        {
            CantidadNovelasIncompletas = ar.ObtenTodasNovelas().ToList().Where(x => x.EstoyCompleta == false).Count();
        }

        /// <summary>
        /// Este event handler actualiza los numeros en pantalla.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ProgresoNovela_ProgressChanged(object sender, IReporte e)
        {
            string estado = e.PorcentajeDeCompletado < 100 ? "Descargando" : "Completado";

            foreach (ReporteWPF reporte in DescargasNovelas)
            {
                if (reporte.NombreItem.Equals(e.NombreItem))
                {
                    reporte.Total = e.Total;
                    reporte.Actual = e.Actual;
                    reporte.Estado = estado;
                    break;
                }
            }
            ManagerTareas.ActualizaReporte(e);
        }

        /// <summary>
        /// Este event handler le pasa la novela al sistema encargado de descargar novelas.
        /// </summary>
        /// <param name="novela"></param>
        private void GetNovelsWPFEvents_DescargaNovela(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela)
        {
            IngresaNovelaADescargas(novela);
        }


        #endregion
    }
}
