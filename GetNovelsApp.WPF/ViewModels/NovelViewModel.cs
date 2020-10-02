using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
            ActualizaImagen();

            Capitulos = new ObservableCollection<Capitulo>(NovelaEnVista.Capitulos);
            Novelas.Add(NovelaEnVista);
        }


        private NovelaWPF novelaEnVista;
        private string pathImagenNovela;

        public NovelaWPF NovelaEnVista { get => novelaEnVista; set => OnPropertyChanged(ref novelaEnVista, value); }

        public string PathImagenNovela { get => pathImagenNovela; private set => OnPropertyChanged(ref pathImagenNovela, value); }

        public ObservableCollection<NovelaWPF> Novelas { get; private set; } = new ObservableCollection<NovelaWPF>();


        public ObservableCollection<Capitulo> Capitulos { get; private set; } = new ObservableCollection<Capitulo>();

        private void ActualizaImagen()
        {
            PathImagenNovela = EncontradorImagen.DescargaImagen(NovelaEnVista.ImagenLink.ToString());
        }

    }
}
