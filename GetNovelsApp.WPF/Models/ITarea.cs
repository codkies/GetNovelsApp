namespace GetNovelsApp.WPF.Models
{
    public interface ITarea
    {
        int ID { get; }


        string Estado { get; } //Descargando, guardando, etc...


        string Item { get; } //Nombre de novela, cap, etc...


        int PorcentajeTarea { get; }


        bool ActualizaProgreso(int tareaID, int progreso, string estado);

    }
}
