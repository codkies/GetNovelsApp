namespace GetNovelsApp.WPF.Models
{
    public interface ITarea 
    {
        string Estado { get; } //Descargando, guardando, etc...


        string NombreItem { get; } //Nombre de novela, cap, etc...

        int PorcentajeTarea { get; }


        void ActualizaProgreso(int progreso, string estado);

    }
}
