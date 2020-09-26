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
        private static void DestruyeReferencias()
        {
            ImprimeNovela = null;
        }


        public static event Action<Novela, TiposDocumentos> ImprimeNovela;


        public static void Invoke_ImprimeNovela(Novela novela, TiposDocumentos tipo)
        {
            ImprimeNovela?.Invoke(novela, tipo);
        }

    }
}
