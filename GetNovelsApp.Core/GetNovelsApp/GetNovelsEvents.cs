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

    }
}
