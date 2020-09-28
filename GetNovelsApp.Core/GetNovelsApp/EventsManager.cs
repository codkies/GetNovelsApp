using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GetNovelsApp.Core.Empaquetadores;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core
{
    public static class EventsManager
    {
        /// <summary>
        /// Limpia los eventos.
        /// </summary>
        public static void DestruyeReferencias()
        {
            ImprimeNovela = null;
        }


        public static event Action<NovelaRuntimeModel, TiposDocumentos> ImprimeNovela;

        /// <summary>
        /// Llamado cuando se desea que se imprima una novela.
        /// </summary>
        /// <param name="novela"></param>
        /// <param name="tipo"></param>
        public static void Invoke_ImprimeNovela(NovelaRuntimeModel novela, TiposDocumentos tipo)
        {
            ImprimeNovela?.Invoke(novela, tipo);
        }

    }
}
