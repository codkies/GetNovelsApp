using System;
using System.Collections.Generic;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace GetNovelsApp.Core.Modelos
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">IEnumerable de capitulos</typeparam>
    /// <typeparam name="Y">IEnumerable de strings</typeparam>
    /// <typeparam name="R">IEnumerable de Uri</typeparam>
    public interface INovela<out T, out Y, out R> 
        where T : IEnumerable<Capitulo>
        where Y : IEnumerable<string>
        where R : IEnumerable<Uri>
    {
        //Main
        string Titulo { get; }
        string Autor { get; }
        bool HistoriaCompleta { get; }
        bool TraduccionCompleta { get; }
        float Review { get; }
        int CantidadReviews { get; }        
        int ID { get; }
        T Capitulos { get; }
        T CapitulosDescargados { get; }
        T CapitulosImpresos { get; }
        T CapitulosPorDescargar { get; }
        public Y Tags { get; }
        public Y Generos { get; }
        public string Sipnosis { get; }
        public Uri ImagenLink { get; }


        //Estado de la clase
        bool EstoyCompleta { get; }        
        Uri LinkPrincipal { get; }
        R LinksDeCapitulos { get; }
        int PorcentajeDescarga { get; }
        bool TengoCapitulosPorImprimir { get; }
        int CantidadCapitulosDescargados { get; }
        int CantidadLinks { get; }

        //Metodos
        void CapituloFueDescargado(Capitulo capituloNuevo);
        void CapituloFueImpreso(Capitulo capitulo);
    }
}