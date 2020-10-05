using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.Core.Conexiones.DB;
using GetNovelsApp.Core.Modelos;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;
using Testing;

namespace GetNovelsApp.WPF.ViewModels
{
    public class DescargasViewModel : ObservableObject
    {
        #region Cons fields

        /// <summary>
        /// Referencia a la app.
        /// </summary>
        private readonly GetNovels GetNovels;
        private ObservableCollection<Descarga> descargas = new ObservableCollection<Descarga>();

        #endregion

        #region Ctors

        public DescargasViewModel(GetNovels getNovels)
        {
            GetNovels = getNovels;
            GetNovelsWPFEvents.DescargaNovela += GetNovelsWPFEvents_DescargaNovela;
            //obtenNovelasDemo();
        }

        private async Task obtenNovelasDemo()
        {
            var novelas = await new Archivador().ObtenTodasNovelasAsync();
            foreach (var nov in novelas)
            {
                Descargas.Add( new Descarga(nov)); 
            }
        }

        #endregion

        #region Props 


        public ObservableCollection<Descarga> Descargas { get => descargas; set => OnPropertyChanged(ref descargas, value); }


        #endregion

        #region Metodos

        private async void GetNovelsWPFEvents_DescargaNovela(INovela novela)
        {
            Progress<Descarga> Reporte = new Progress<Descarga>();

            Descarga descarga = new Descarga(novela);

            Descargas.Add(descarga);

            Reporte.ProgressChanged += Reporte_ProgressChanged;

            bool exito = await GetNovels.AgregaAlQueue(novela, Reporte);
            if(exito == false)
            {
                throw new NotImplementedException($"no se pudo agregar {novela.Titulo} a las descargas.");
            }
        }

        /// <summary>
        /// Llamado cada vez que un reporte creado por este script, cambia.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Reporte_ProgressChanged(object sender, Descarga e)
        {
            //Encontrando la novela a la que este cambio está notificando.
            var descarga = Descargas.Where(i => i.Novela == e.Novela).First();

            descarga.PorcentajeDescarga = e.PorcentajeDescarga;
        }


        #endregion



        #region Commands

        public void PausaDescargas()
        {

        }

        public void ComienzaDescargas()
        {

        }

        #endregion

    }
}
