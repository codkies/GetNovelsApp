using System.Drawing;

namespace GetNovelsApp.Core.Conexiones.DB
{
    public struct InformacionNovelaDB
    {
        public string Titulo { get; set; }

        public string LinkPrincipal { get; set; }

        public int ID { get; set; }

        public string Sipnosis { get; set; }

        public string Tags { get; set; } //Crear alguna función que los separe por comas

        public Image Imagen { get; set; } //Buscar como sacar y meter imagenes de la DB
        
    }
}
