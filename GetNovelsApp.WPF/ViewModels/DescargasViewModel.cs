using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core;
using GetNovelsApp.WPF.Models;
using GetNovelsApp.WPF.Utilidades;

namespace GetNovelsApp.WPF.ViewModels
{
    public enum EstadoDescarga
    {
        Descargando,
        Pausa,
        Fallida
    }

    public class Descarga
    {
        public NovelaWPF Novela { get; set; }


        public int PorcentajeDescarga { get; set; }


        public EstadoDescarga EstadoDescarga { get; set; }


        public bool Completa => PorcentajeDescarga == 100;
    }

    public class DescargasViewModel 
    {
        #region Cons fields

        /// <summary>
        /// Referencia a la app.
        /// </summary>
        private readonly GetNovels GetNovels;

        #endregion

        #region Ctors

        public DescargasViewModel(GetNovels getNovels)
        {
            GetNovels = getNovels;
            GetNovelsWPFEvents.AgregaDescarga += AgregaDescarga;
        }

        #endregion

        #region Props

        public List<Descarga> Descargas { get; set; }

        #endregion

        #region Metodos

        private void AgregaDescarga(Descarga descarga)
        {
            Descargas.Add(descarga);
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
