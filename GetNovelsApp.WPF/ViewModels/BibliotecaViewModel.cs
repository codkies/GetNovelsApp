using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;
using GetNovelsApp.WPF.Views;

namespace GetNovelsApp.WPF.ViewModels
{
    public class BibliotecaViewModel : ObservableObject
    {
        private readonly Archivador ar;
        public BibliotecaViewModel()
        {
            ar = new Archivador();
            GetNovelsEvents.NovelaAgregadaADB += GetNovelsEvents_NovelaAgregadaADB;
            Command_VerNovela = new RelayCommand<NovelaWPF>(VerNovela, Puedo_VerNovela);
            Command_AgregarNovela = new RelayCommand(AgregarNovela, Puedo_AgregarNovela);
            if (NovelasVisibles == null & isExecuting == false) ObtenNovelasDB();
        }

        /// <summary>
        /// Define si el Async para obtener las novelas a mostrar en la biblioteca está corriendo.
        /// </summary>
        bool isExecuting = false;

        private async void GetNovelsEvents_NovelaAgregadaADB()
        {
            await ObtenNovelasDB();
        }

        private async Task ObtenNovelasDB()
        {
            Debug.WriteLine("Buscando novelas en DB");
            isExecuting = true;
            var novelasEnDB = await ar.ObtenTodasNovelasAsync();

            List<NovelaWPF> Novelas = new List<NovelaWPF>();
            foreach (INovela novela in novelasEnDB)
            {
                Novelas.Add((NovelaWPF)novela);
            }

            NovelasVisibles = new ObservableCollection<NovelaWPF>(Novelas);
            isExecuting = false;
            Debug.WriteLine("Novelas en DB, encontradas.");
        }


        #region Propiedades de la view


        private ObservableCollection<NovelaWPF> novelasVisibles;


        public ObservableCollection<NovelaWPF> NovelasVisibles { get => novelasVisibles; set => OnPropertyChanged(ref novelasVisibles, value); }

        #endregion


        #region Ver novela


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


        #region Agregar novela

        public RelayCommand Command_AgregarNovela { get; set; }

        public void AgregarNovela()
        {
            AddNovelView addNovelView = new AddNovelView();
            addNovelView.Show();

        }

        public bool Puedo_AgregarNovela()
        {
            return true;
        }

        #endregion

    }
}
