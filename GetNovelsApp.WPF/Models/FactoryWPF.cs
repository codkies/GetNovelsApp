using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;

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

        #region Capitulos

        public async Task<List<Capitulo>> FabricaCapitulos(List<Uri> ListaDeLinks)
        {
            List<Capitulo> Capitulos = new List<Capitulo>();
            
            for (int i = 0; i < ListaDeLinks.Count; i++)
            {
                Uri link = ListaDeLinks[i];
                CapituloWebModel _ = new CapituloWebModel(link, $"Chapter {i + 1}", i + 1);
                Capitulo capitulo = new Capitulo(_);
                Capitulos.Add(capitulo);
            }
            await Task.Delay(100);
            return Capitulos;
        }

        #endregion


        #region Configuracion


        public IConfig FabricaConfiguracion(string Path, int TamañoBatch, int CapsPorDoc)
        {
            return new ConfiguracionBasica(TamañoBatch, CapsPorDoc, Path);
        }

        #endregion


        #region Constructores

        public IConstructor FabricaConstructor(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, TiposDocumentos tipo, int capsPorPDF, string direccion, string titulo, NotificaCapituloImpreso notCapImpreso, NotificaDocumentoCreado notDocCreado)
        {
            switch (tipo)
            {
                case TiposDocumentos.PDF:
                    return new ConstructorPDF(novela, capsPorPDF, direccion, titulo, notCapImpreso, notDocCreado);
                default:
                    throw new NotImplementedException("No se han creado constructores para otros tipos de archivos que no sean PDF.");
            }
        }

        #endregion


        #region Novelas

        public INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> FabricaNovela(List<Capitulo> capitulos, InformacionNovelaDB dbInfo)
        {
            return new NovelaWPF(capitulos, dbInfo);
        }

        #endregion


        #region Website perfil

        public IPath FabricaWebsite(string dominio, List<string> xpathsLinks, List<string> xpathsTextos, List<string> xpathsTitulo, OrdenLinks OrdenLinks)
        {
            return new Website(dominio, xpathsLinks, xpathsTextos, xpathsTitulo, OrdenLinks);
        }

        #endregion

        #region Reporte

        public IReporte FabricaReporteNovela(int total, int actual, string estado, int identificador, IReportero reportero, string nombreItem)
        {
            return new ReporteWPF(total , actual, estado, identificador, reportero, nombreItem);
        }

        #endregion
    }
}
