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
            WebsitesCambiaron = null;
            NovelaAgregadaADB = null;
        }


        #region Imprime novela
        public static event Action<INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>>, TiposDocumentos> ImprimeNovela;

        /// <summary>
        /// Llamado cuando se desea que se imprima una novela.
        /// </summary>
        /// <param name="novela"></param>
        /// <param name="tipo"></param>
        public static void Invoke_ImprimeNovela(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, TiposDocumentos tipo)
        {
            ImprimeNovela?.Invoke(novela, tipo);
        }
        #endregion


        #region Configuracion cambio
        public static event Action ConfiguracionCambio;

        /// <summary>
        /// Notifica a la app de que la configuración (en DB) ha cambiado.
        /// </summary>
        public static void Invoke_ConfiguracionCambio()
        {
            ConfiguracionCambio?.Invoke();
        }
        #endregion


        #region Perfiles de Websites cambiaron
        public static event Action WebsitesCambiaron;
        /// <summary>
        /// Notifica a la app de que la configuración (en DB) ha cambiado.
        /// </summary>
        public static void Invoke_WebsitesCambiaron()
        {
            WebsitesCambiaron?.Invoke();
        }
        #endregion


        #region Novela agregada

        public static event Action NovelaAgregadaADB;

        public static void Invoke_NovelaAgregadaADB()
        {
            NovelaAgregadaADB?.Invoke();
        }

        #endregion
    }
}
