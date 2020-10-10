using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.Core
{
    /// <summary>
    /// Lugar de donde salen todas las instancias.
    /// </summary>
    public static class GetNovelsFactory
    {
        private static IFabrica Fabrica;

        public static void InicializaFabrica(IFabrica fabrica)
        {
            Fabrica = fabrica;
        }


        public static IPath FabricaWebsite(string dominio, List<string> xpathsLinks, List<string> xpathsTextos, List<string> xpathsTitulo, OrdenLinks OrdenLinks)
        {
            return Fabrica.FabricaWebsite(dominio, xpathsLinks, xpathsTextos, xpathsTitulo, OrdenLinks);
        }


        /// <summary>
        /// Toma una lista de Uri's y las convierte en capitulos (con la ayuda del Manipulador de Links).
        /// </summary>
        /// <param name="ListaDeLinks"></param>
        /// <returns></returns>
        public static List<Capitulo> FabricaCapitulos(List<Uri> ListaDeLinks)
        {
            return Fabrica.FabricaCapitulos(ListaDeLinks);
        }


        public static IConfig FabricaConfiguracion(string Path, int TamañoBatch, int CapsPorDoc)
        {
            return Fabrica.FabricaConfiguracion(Path, TamañoBatch, CapsPorDoc);
        }


        /// <summary>
        /// Asigna un constructor dependiendo del tipo de archivo que se desee.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public static IConstructor FabricaConstructor(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, TiposDocumentos tipo, int capsPorPDF, string direccion, string titulo, NotificaCapituloImpreso notCapImpreso, NotificaDocumentoCreado notDocCreado)
        {
            return Fabrica.FabricaConstructor(novela, tipo, capsPorPDF, direccion, titulo, notCapImpreso, notDocCreado);
        }



        public static INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> FabricaNovela(IEnumerable<Capitulo> capitulos, InformacionNovelaDB info)
        {
            List<Capitulo> _ = new List<Capitulo>(capitulos);
            return Fabrica.FabricaNovela(_, info);
        }



        private static int reporteIDContador = 0;
        public static IReporte FabricaReporteNovela(int total, int actual, string estado, IReportero reportero, string nombreItem)
        {
            reporteIDContador++;
            return Fabrica.FabricaReporteNovela(total, actual, estado, reporteIDContador, reportero, nombreItem);
        }

    }

}
