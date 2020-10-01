using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class NovelViewModel : ObservableObject
    {
        public NovelViewModel(NovelaWPF novela)
        {
            novelaEnVista = novela;
        }

      
        private NovelaWPF novelaEnVista;


        public NovelaWPF NovelaEnVista { get => novelaEnVista; set => OnPropertyChanged(ref novelaEnVista, value); }

        public string Sipnosis => novelaEnVista.Sipnosis;

        public Uri Imagen => novelaEnVista.ImagenLink;

        public string Titulo => novelaEnVista.Titulo;

        public List<string> Tags => novelaEnVista.Tags;

    }
}
