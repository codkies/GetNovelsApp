using System.Collections.Generic;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core
{
    /// <summary>
    /// Lugar de donde salen todas las instancias.
    /// </summary>
    public static class GetNovelsFactory
    {
        public static void EstableceConfig(IFabrica fabrica)
        {
            Fabrica = fabrica;
        }

        private static IFabrica Fabrica;


        /// <summary>
        /// Asigna un constructor dependiendo del tipo de archivo que se desee.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        public static IConstructor AsignaConstructor(INovela novela, TiposDocumentos tipo, int capsPorPDF, string direccion, string titulo, NotificaCapituloImpreso notCapImpreso, NotificaDocumentoCreado notDocCreado)
        {
            return Fabrica.FabricaConstructor(novela, tipo, capsPorPDF, direccion, titulo, notCapImpreso, notDocCreado);
        }

        public static IConstructor ObtenNovela(IConstructor IConfig)
        {
            return Fabrica.FabricaConstructor(IConfig);
        }


        public static INovela ObtenNovela(INovela INovela)
        {
            return Fabrica.FabricaNovela(INovela);
        }

        public static INovela ObtenNovela(List<Capitulo> capitulos, InformacionNovelaDB info)
        {
            return Fabrica.FabricaNovela(capitulos, info);
        }
    }

}
