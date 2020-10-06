using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.WebSockets;
using iText.Layout.Element;

namespace GetNovelsApp.Core.Conexiones.DB
{
    public enum EstadoHistoria { Completa, Incompleta }
    public enum EstadoTraduccion { Completa, Incompleta }

    /// <summary>
    /// Modelo de la informacion de una novela en la DB de tablas Novelas y Tags. (Capitulos y sus links no incluidos).
    /// </summary>
    public class InformacionNovelaDB
    {
        public string Titulo { get; set; }

        public string Autores { get; set; }

        public EstadoHistoria EstadoHistoria {get; set;}

        public string LinkPrincipal { get; set; }

        public int ID { get; set; }

        public string Sipnosis { get; set; }
       
        public string Tags { get; set; } //Crear alguna función que los separe por comas

        public string Generos { get; set; } //Crear alguna función que los separe por comas

        public string Imagen { get; set; } //Buscar como sacar y meter imagenes de la DB
    }
}
