using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.WPF.Models
{
    /// <summary>
    /// Por configurar.
    /// </summary>
    public class FactoryWPF : IFabrica
    {

        public FactoryWPF()
        {

        }

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
            return new NovelaWPF(capitulos, dbInfo);
        }

        public INovela FabricaNovela(INovela novela)
        {
            throw new NotImplementedException("Implementa la conversion de novelas.");
        }
    }
}
