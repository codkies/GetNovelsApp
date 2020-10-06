﻿using System;
using System.Collections.Generic;

namespace GetNovelsApp.Core.Modelos
{
    public interface INovela
    {
        //Main
        string Titulo { get; }
        List<string> Autores { get; }
        bool HistoriaCompleta { get; }
        float Review { get; }
        float CantidadReviews { get; }
        bool TraduccionCompleta { get; }
        int CantidadCapitulosDescargados { get; }
        int ID { get; }
        List<Capitulo> Capitulos { get; }
        List<Capitulo> CapitulosDescargados { get; }
        List<Capitulo> CapitulosImpresos { get; }
        List<Capitulo> CapitulosPorDescargar { get; }
        public List<string> Tags { get; }
        public List<string> Generos { get; }
        public string Sipnosis { get; }
        public Uri ImagenLink { get; }


        //Estado de la clase
        bool EstoyCompleta { get; }        
        Uri LinkPrincipal { get; }
        List<Uri> LinksDeCapitulos { get; }
        int PorcentajeDescarga { get; }
        bool TengoCapitulosPorImprimir { get; }
        int CantidadLinks { get; }

        //Metodos
        void CapituloFueDescargado(Capitulo capituloNuevo);
        void CapituloFueImpreso(Capitulo capitulo);
    }
}