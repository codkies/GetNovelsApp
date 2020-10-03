using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core
{
    public static class GetNovelsEvents
    {
        /// <summary>
        /// Limpia los eventos.
        /// </summary>
        public static void DestruyeReferencias()
        {
            ImprimeNovela = null;
            ConfiguracionCambio = null;
        }


        public static event Action<INovela, TiposDocumentos> ImprimeNovela;

        /// <summary>
        /// Llamado cuando se desea que se imprima una novela.
        /// </summary>
        /// <param name="novela"></param>
        /// <param name="tipo"></param>
        public static void Invoke_ImprimeNovela(INovela novela, TiposDocumentos tipo)
        {
            ImprimeNovela?.Invoke(novela, tipo);
        }


        public static event Action ConfiguracionCambio;


        /// <summary>
        /// Notifica a la app de que la configuración (en DB) ha cambiado.
        /// </summary>
        public static void Invoke_ConfiguracionCambio()
        {
            ConfiguracionCambio?.Invoke();
        }

        public static event Action WebsitesCambiaron;


        /// <summary>
        /// Notifica a la app de que la configuración (en DB) ha cambiado.
        /// </summary>
        public static void Invoke_WebsitesCambiaron()
        {
            WebsitesCambiaron?.Invoke();
        }


    }
}
