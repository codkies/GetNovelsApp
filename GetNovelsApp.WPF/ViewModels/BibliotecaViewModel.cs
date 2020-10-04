using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;
using GetNovelsApp.WPF.Views;

namespace GetNovelsApp.WPF.ViewModels
{
    public class BibliotecaViewModel : ObservableObject
    {

        public BibliotecaViewModel(List<NovelaWPF> NovelasEnBiblioteca)
        {
            NovelasVisibles = new ObservableCollection<NovelaWPF>(NovelasEnBiblioteca);
            Command_VerNovela = new RelayCommand<NovelaWPF>(VerNovela, Puedo_VerNovela);
        }


        #region Propiedades de la view


        private ObservableCollection<NovelaWPF> novelasVisibles;

        public ObservableCollection<NovelaWPF> NovelasVisibles { get => novelasVisibles; set => OnPropertyChanged(ref novelasVisibles, value); } 


        public RelayCommand<NovelaWPF> Command_VerNovela { get; set; }

        public void VerNovela(NovelaWPF novela)
        {          
            //Crea un ViewModel para la novela, y pasaselo al AppViewModel (main script).
            GetNovelsWPFEvents.Invoke_Cambia(new NovelViewModel(novela));
        }

        public bool Puedo_VerNovela(NovelaWPF novela)
        {
            return true;
        }

        #endregion
    }
}
