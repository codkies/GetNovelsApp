﻿using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Empaquetador;
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


        public void DescargaNovelaAsync()
        {
            GetNovelsWPFEvents.Invoke_DescargaNovela(NovelaEnVista);
            NovelaEnVista.Descargando = true;
        }

        public bool Puedo_DescargaNovela()
        {
            return NovelaEnVista.EstoyCompleta == false & 
                   NovelaEnVista.Descargando == false;
        }

        #endregion


        #region Leer

        public RelayCommand Command_Leer { get; set; }

        bool Leyendo = false;
        private async void Leer()
        {
            int primerCap = NovelaEnVista.CapitulosDescargados.Count > 0 ? 
                            (int)NovelaEnVista.CapitulosDescargados.First().NumeroCapitulo : 
                            (int)NovelaEnVista.CapitulosImpresos.First().NumeroCapitulo;

            int ultimoCap = NovelaEnVista.CapitulosDescargados.Count > 0 ?
                            (int)NovelaEnVista.CapitulosDescargados.Last().NumeroCapitulo : 
                            (int)NovelaEnVista.CapitulosImpresos.Last().NumeroCapitulo;


            var locacion = LocalPathManager.DefinePathNovela(NovelaEnVista, primerCap, ultimoCap);

            if (File.Exists(locacion))
            {
                AbreCarpetaDescargas();
            }
            else
            {
                Leyendo = true;
                ReporteWPF reporte = (ReporteWPF)GetNovelsFactory.FabricaReporteNovela(NovelaEnVista.CapitulosDescargados.Count, 0,
                                                                                       "Guardando PDF", this, NovelaEnVista.Titulo);
                ManagerTareas.MuestraReporte(reporte);

                Progreso progreso = new Progreso();
                progreso.ProgressChanged += Progreso_ProgressChanged;
                await AppViewModel.GetNovels.MyEmpaquetador.ImprimeNovela(NovelaEnVista, TiposDocumentos.PDF, progreso);
                Leyendo = false;
            }
        }

       

        private bool Puede_Leer()
        {
            bool capitulosOBtenidos = NovelaEnVista.CantidadCapitulosDescargados > 1 | NovelaEnVista.CapitulosImpresos.Count > 1;
            return capitulosOBtenidos & 
                   Leyendo == false;
        }

        #endregion

        #region event handler

        private void Progreso_ProgressChanged(object sender, IReporte e)
        {
            if(e.PorcentajeDeCompletado >= 100)
            {
                AbreCarpetaDescargas();
            }
            ManagerTareas.ActualizaReporte(e);
        }

        private void AbreCarpetaDescargas()
        {
            try
            {
                var locacion = GetNovelsConfig.HardDrivePath;
                Process.Start($@"{locacion}");
            }
            catch (Win32Exception win32Exception)
            {
                //The system cannot find the file specified...
                Debug.WriteLine(win32Exception.Message);
            }
        }


        #endregion

    }
}
