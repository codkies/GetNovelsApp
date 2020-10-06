using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.Models
{
    public class Tarea : ObservableObject, ITarea
    {
        private string estado;
        private string item;
        private int porcentajeTarea;


        public Tarea(string estado, string item, int porcentajeTarea, int id)
        {
            Estado = estado;
            Item = item;
            PorcentajeTarea = porcentajeTarea;
            ID = id;
        }


        /// <summary>
        /// Regresa true si la tarea se pudo actualizar. Los ID tienen que ser iguales.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public bool ActualizaProgreso(int tareaID, int progreso, string estado)
        {
            if (ID != tareaID) return false;

            PorcentajeTarea = progreso;
            Estado = estado;
            return true;
        }

        /// <summary>
        /// Identificador de esta tarea.
        /// </summary>
        public int ID { get; private set; }


        /// <summary>
        /// El estado de la tarea. "Descargando", "Pausa", etc...
        /// </summary>
        public string Estado { get => estado; set => OnPropertyChanged(ref estado, value); }


        /// <summary>
        /// El item sobre el que trata esta tarea.
        /// </summary>
        public string Item { get => item; set => OnPropertyChanged(ref item, value); }


        public int PorcentajeTarea { get => porcentajeTarea; set => OnPropertyChanged(ref porcentajeTarea, value); }
    }
}
