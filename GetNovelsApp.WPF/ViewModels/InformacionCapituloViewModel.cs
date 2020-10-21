using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public class InformacionCapituloViewModel : ObservableObject
    {
        public event Action<CapituloWebModel> InformacionObtenida;
        private string linkCapitulo;
        private string titulo;
        private string valor;
        private string numero;

        public InformacionCapituloViewModel(string linkCapitulo)
        {
            LinkCapitulo = linkCapitulo;
            Command_Confirmar = new RelayCommand(Confirmar, Puede_Confimar);
        }

        public string Titulo { get => titulo; set => OnPropertyChanged(ref titulo, value); }

        public string LinkCapitulo { get => titulo; set => OnPropertyChanged(ref linkCapitulo, value); }

        public string Valor { get => valor; set => OnPropertyChanged(ref valor, value); }

        public string Numero { get => numero; set => OnPropertyChanged(ref numero, value); }



        #region Comando, confimar

        public RelayCommand Command_Confirmar { get; set; }

        private void Confirmar()
        {
            CapituloWebModel info = new CapituloWebModel(new Uri(LinkCapitulo), Titulo, int.Parse(Valor) ,int.Parse(Numero));
            InformacionObtenida?.Invoke(info);
        }

        private bool Puede_Confimar()
        {
            bool tituloEscrito = string.IsNullOrEmpty(Titulo) == false;
            bool valorEscrito = string.IsNullOrEmpty(Valor) == false;
            bool numeroEscrito = string.IsNullOrEmpty(numero) == false;

            return tituloEscrito & valorEscrito & numeroEscrito;

        }

        #endregion

    }
}
