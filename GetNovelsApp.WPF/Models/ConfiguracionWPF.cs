using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.ConfiguracionApp;
using GetNovelsApp.Core.ConfiguracionApp.xPaths;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Reportaje;

namespace GetNovelsApp.WPF.Models
{
    public class ConfiguracionWPF : IConfig
    {

        public ConfiguracionWPF(IPath paths, IFabrica fabrica, IComunicador comunicador, string folderPath, int tamañoBatch, int capitulosPorDocumento)
        {
            Paths = paths;
            Fabrica = fabrica;
            Comunicador = comunicador;
            FolderPath = folderPath;
            TamañoBatch = tamañoBatch;
            CapitulosPorDocumento = capitulosPorDocumento;
        }


        public IPath Paths { get; private set; }

        public IFabrica Fabrica { get; private set; }

        public IComunicador Comunicador { get; private set; }

        public string FolderPath { get; private set; }

        public int TamañoBatch { get; private set; }

        public int CapitulosPorDocumento { get; private set; }



        public List<string> xPathsTextos => Paths.xPathsTextos;

        public List<string> xPathsSiguienteBoton => Paths.xPathsSiguienteBoton;

        public List<string> xPathsTitulo => Paths.xPathsTitulo;

        public List<string> xPathsLinks => Paths.xPathsLinks;
    }
}