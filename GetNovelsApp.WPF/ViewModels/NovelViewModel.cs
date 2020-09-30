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
            Tags = ConvierteEnTags(novela);
        }

        private List<string> ConvierteEnTags(NovelaWPF novela)
        {
            throw new NotImplementedException("Crear alguna función que los separe por comas");
        }

        private NovelaWPF novelaEnVista;
        List<string> tags;

        /// <summary>
        /// Novela visible.
        /// </summary>
        public NovelaWPF NovelaEnVista { get => novelaEnVista; set => OnPropertyChanged(ref novelaEnVista, value); }

        public string Sipnosis => novelaEnVista.Sipnosis;

        public Image Imagen => novelaEnVista.Imagen;

        public string Titulo => novelaEnVista.Titulo;

        public List<string> Tags { get => tags; set => OnPropertyChanged(ref tags, value); }

    }
}
