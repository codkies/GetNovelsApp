using System;
using System.Threading;
using System.Threading.Tasks;
using GetNovelsApp.Core.Conexiones.Internet;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.WPF.ViewModels;
using GetNovelsApp.WPF.Views;

namespace GetNovelsApp.WPF.Models
{
    internal class ComunicadorWPF : IComunicador
    {
        #region Pidiendo info capitulo

        private CapituloWebModel InformacionCapitulo;

        public async Task<CapituloWebModel> PideInfoCapituloAsync(string linkCapitulo)
        {
            //View
            InformacionCapituloView informacionCapituloView = new InformacionCapituloView();
            //ViewModel
            InformacionCapituloViewModel controlador = new InformacionCapituloViewModel(linkCapitulo);
            //Notifica cuando el usuario termine de escribir la info
            controlador.InformacionObtenida += Controlador_InformacionObtenida;
            //Conecta la View con el ViewModel
            informacionCapituloView.DataContext = controlador;
            //Muestra la ventana
            informacionCapituloView.Show();            

            //Espera hasta que el usuario introduzca la informacion
            while(InformacionCapitulo == null)
            {
                await Task.Delay(5 * 1000); //5s
            }
            //Cierra la ventana
            informacionCapituloView.Close();
            //end
            return InformacionCapitulo;
        }

        private void Controlador_InformacionObtenida(CapituloWebModel obj)
        {
            InformacionCapitulo = obj;
        }



        #endregion


        public string PideInput(string enunciado, IReportero reportero)
        {
            throw new System.NotImplementedException();
        }

        public void Reporta(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Reporte\n {enunciado}");
        }

        public void ReportaCambioEstado(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Cambio de estado\n {enunciado}");
        }

        public void ReportaError(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Error {Thread.CurrentThread.Name}\n {enunciado}");
        }

        public void ReportaErrorMayor(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Error grande {Thread.CurrentThread.Name}\n {enunciado}");
        }

        public void ReportaEspecial(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Especial\n {enunciado}");
        }

        public void ReportaExito(string enunciado, IReportero reportero)
        {
            System.Diagnostics.Debug.WriteLine($"{reportero.Nombre}: Exito\n {enunciado}");
        }
    }
}