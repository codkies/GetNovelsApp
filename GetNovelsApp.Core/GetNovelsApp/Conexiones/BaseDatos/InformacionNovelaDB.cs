using System;
using System.Collections.Generic;
using System.Drawing;
using System.Web.WebSockets;
using iText.Layout.Element;

namespace GetNovelsApp.Core.Conexiones.DB
{
    public class InformacionNovelaDB
    {
        //public InformacionNovelaDB(string titulo, string linkPrincipal, int iD, string sipnosis, string tags, string imagen)
        //{
        //    Titulo = titulo;
        //    LinkPrincipal = linkPrincipal;
        //    ID = iD;
        //    Sipnosis = sipnosis;
        //    Tags = tags;
        //    Imagen = imagen;
        //}

        //public InformacionNovelaDB(string titulo, string linkPrincipal, int iD, string sipnosis, string tags, string imagen)
        //{
        //    Titulo = titulo;
        //    LinkPrincipal = linkPrincipal;
        //    ID = iD;
        //    Sipnosis = sipnosis;
        //    Tags = tags;
        //    Imagen = imagen;
        //}


        //Basicos:
        public string Titulo { get; set; }

        public string LinkPrincipal { get; set; }

        public int ID { get; set; }

        public string Sipnosis { get; set; }
       
        public string Tags { get; set; } //Crear alguna función que los separe por comas

        public string Imagen { get; set; } //Buscar como sacar y meter imagenes de la DB
    }
}
