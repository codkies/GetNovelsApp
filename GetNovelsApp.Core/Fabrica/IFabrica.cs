using System.Collections.Generic;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.GetNovelsApp
{
    public interface IFabrica
    {
        public INovela FabricaNovela(List<Capitulo> capitulos, InformacionNovelaDB dbInfo);

        public INovela FabricaNovela(INovela novela);

        public IConstructor FabricaConstructor(INovela novela, TiposDocumentos tipo, int capsPorPDF, string direccion, string titulo, NotificaCapituloImpreso notCapImpreso, NotificaDocumentoCreado notDocCreado);

        public IConstructor FabricaConstructor(IConstructor constructor);
    }
}
