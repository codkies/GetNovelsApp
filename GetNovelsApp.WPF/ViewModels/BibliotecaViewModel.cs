using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class BibliotecaViewModel : ObservableObject
    {


        #region Propiedades de la view


        private ObservableCollection<NovelaWPF> novelasVisibles;

        public ObservableCollection<NovelaWPF> NovelasVisibles { get => novelasVisibles; set => OnPropertyChanged(ref novelasVisibles, value); } 



        #endregion
    }
}
