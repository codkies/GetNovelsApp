using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class NovelViewModel : ObservableObject
    {
        public NovelViewModel(NovelaWPF novela)
        {            
            NovelaEnVista = novela;
            if(novela.ImagenLink != null) ActualizaImagen();

            Command_DescargaNovela = new RelayCommand(DescargaNovelaAsync, Puedo_DescargaNovela);
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

        public async void DescargaNovelaAsync()
        {
            Debug.WriteLine($"Descargando {NovelaEnVista.Titulo}");
            await AppViewModel.GetNovels.GetNovelAsync(NovelaEnVista, 0);
            Debug.WriteLine($"Descargada {NovelaEnVista.Titulo}");
        }

        public bool Puedo_DescargaNovela()
        {
            return !NovelaEnVista.EstoyCompleta;
        }

        #endregion



    }
}
