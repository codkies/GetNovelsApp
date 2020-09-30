using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.Models
{
    class NovelaWPFModel : ObservableObject, INovela
    {
        public NovelaWPFModel(INovela novela)
        {
            //Ctor para construtir una de estas a partir de lo que envie la DB.
        }


        #region Identificadores y fields clave

        private string titulo;
        private int cantidadCapitulosDescargados;
        private List<Capitulo> capitulosDescargados;
        private List<Capitulo> capitulosImpresos;
        private List<Capitulo> capitulosPorDescargar;

        public string Titulo
        {
            get => titulo;
            set => OnPropertyChanged(ref titulo, value);
        }

        public int CantidadCapitulosDescargados
        {
            get => cantidadCapitulosDescargados;
            set => OnPropertyChanged(ref cantidadCapitulosDescargados, value);
        }

        public int ID { get; private set; }

        public List<Capitulo> CapitulosDescargados
        {
            get => capitulosDescargados;
            set => OnPropertyChanged(ref capitulosDescargados, value);
        }

        public List<Capitulo> CapitulosImpresos
        {
            get => capitulosImpresos;
            set => OnPropertyChanged(ref capitulosImpresos, value);
        }

        public List<Capitulo> CapitulosPorDescargar
        {
            get => capitulosPorDescargar;
            set => OnPropertyChanged(ref capitulosPorDescargar, value);
        }

        #endregion


        #region Externos (por implementar)

        public List<Capitulo> Capitulos => throw new NotImplementedException();

        public bool EstoyCompleta => throw new NotImplementedException();

        public Uri LinkPrincipal => throw new NotImplementedException();

        public List<Uri> LinksDeCapitulos => throw new NotImplementedException();

        public int PorcentajeDescarga => throw new NotImplementedException();

        public bool TengoCapitulosPorImprimir => throw new NotImplementedException();

        public int CantidadLinks => throw new NotImplementedException();

        #endregion


        #region Cambio de estado (por implementar)

        public void CapituloFueDescargado(Capitulo capituloNuevo)
        {
            throw new NotImplementedException();
        }

        public void CapituloFueImpreso(Capitulo capitulo)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
