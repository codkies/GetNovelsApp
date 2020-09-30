using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace GetNovelsApp.Core.Modelos
{
    public interface INovela
    {
        int CantidadCapitulosDescargados { get; }
        int CantidadLinks { get; }
        List<Capitulo> Capitulos { get; }
        ReadOnlyCollection<Capitulo> CapitulosDescargados { get; }
        ReadOnlyCollection<Capitulo> CapitulosImpresos { get; }
        ReadOnlyCollection<Capitulo> CapitulosPorDescargar { get; }
        bool EstoyCompleta { get; }
        int ID { get; }
        Uri LinkPrincipal { get; }
        List<Uri> LinksDeCapitulos { get; }
        int PorcentajeDescarga { get; }
        bool TengoCapitulosPorImprimir { get; }
        string Titulo { get; }

        void CapituloFueDescargado(Capitulo capituloNuevo);
        void CapituloFueImpreso(Capitulo capitulo);
    }
}