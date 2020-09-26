using System.Collections.Generic;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores
{
    public delegate void NotificaCapituloImpreso(Capitulo cap, Novela novela);
    public delegate void NotificaDocumentoCreado(string tituloDocumento);

    /// <summary>
    /// Constructor basico que implementa IConstructor
    /// </summary>
    public abstract class ConstructorBasico : IConstructor
    {
        protected abstract int CapitulosPorDoc { get; }
        protected abstract string Path { get; }
        protected abstract string TituloNovela { get; }

        /// <summary>
        /// Notifica que un capitulo fue impreso a quien sea quien creo esta clase.
        /// </summary>
        protected abstract NotificaCapituloImpreso CapituloImpreso { get; }

        /// <summary>
        /// Notifica que un documento fue impreso a quien sea quien creo esta clase.
        /// </summary>
        protected abstract NotificaDocumentoCreado DocumentoCreado { get; }



        /// <summary>
        /// Construye un documento.
        /// </summary>
        /// <param name="notificador">Delegado opcional que notifica cuando un documento ha sido creado.</param>
        public abstract void ConstruyeDocumento(List<Capitulo> CapitulosAImprimir);
    }
}