using System;
using System.Collections.Generic;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.GetNovelsApp
{
    public interface IFabrica
    {
        public INovela FabricaNovela(List<Capitulo> capitulos, InformacionNovelaDB dbInfo);

        public IConstructor FabricaConstructor(INovela novela, TiposDocumentos tipo, int capsPorPDF, string direccion, string titulo, NotificaCapituloImpreso notCapImpreso, NotificaDocumentoCreado notDocCreado);

        public IConfig FabricaConfiguracion(string Path, int TamañoBatch, int CapsPorDoc);

        List<Capitulo> FabricaCapitulos(List<Uri> ListaDeLinks);

        public IPath FabricaWebsite(string dominio, List<string> xpathsLinks, List<string> xpathsTextos, List<string> xpathsTitulo, OrdenLinks OrdenLinks);
    }
}
