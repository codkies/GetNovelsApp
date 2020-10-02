using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.GetNovelsApp
{
    public class FabricaBasica : IFabrica
    {
        public IConstructor FabricaConstructor(INovela novela, TiposDocumentos tipo, int capsPorPDF, string direccion, string titulo, NotificaCapituloImpreso notCapImpreso, NotificaDocumentoCreado notDocCreado)
        {
            switch (tipo)
            {
                case TiposDocumentos.PDF:
                    return new ConstructorPDF(novela, capsPorPDF, direccion, titulo, notCapImpreso, notDocCreado);
                default:
                    throw new NotImplementedException("No se han creado constructores para otros tipos de archivos que no sean PDF.");
            }
        }

        public IConstructor FabricaConstructor(IConstructor constructor)
        {
            throw new NotImplementedException("Implementa la conversion de constructores.");
        }

        public INovela FabricaNovela(List<Capitulo> capitulos, InformacionNovelaDB dbInfo)
        {
            return new NovelaRT(capitulos, dbInfo);
        }

        public INovela FabricaNovela(INovela novela)
        {
            throw new NotImplementedException("Implementa la conversion de novelas.");
        }
    }

}
