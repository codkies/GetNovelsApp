using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.Models
{
    public class Tarea : ObservableObject, ITarea
    {
        private string estado;
        private string item;
        private int porcentajeTarea;


        public Tarea(string estado, string item, int porcentajeTarea)
        {
            Estado = estado;
            NombreItem = item;
            PorcentajeTarea = porcentajeTarea;
        }

        public void ActualizaProgreso(int progreso, string estado)
        {
            PorcentajeTarea = progreso;
            Estado = estado;
        }


        /// <summary>
        /// El estado de la tarea. "Descargando", "Pausa", etc...
        /// </summary>
        public string Estado { get => estado; set => OnPropertyChanged(ref estado, value); }


        /// <summary>
        /// El item sobre el que trata esta tarea.
        /// </summary>
        public string NombreItem { get => item; set => OnPropertyChanged(ref item, value); }


        public int PorcentajeTarea { get => porcentajeTarea; set => OnPropertyChanged(ref porcentajeTarea, value); }
    }
}
