using System;
using System.Collections.Generic;

using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Configuracion;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.CreadorDocumentos.Constructores;

namespace GetNovelsApp.Core.CreadorDocumentos
{
    public enum TiposDocumentos
    {
        PDF,
        TxT,
        Word
    }

    /// <summary>
    /// Encargado de aceptar capitulos e imprimirlos.
    /// </summary>
    public class Empaquetador : IReportero
    {
        #region Constructores & setup

        public Empaquetador(Novela Novela, TiposDocumentos tipo)
        {
            NovelaActual = Novela;
            Constructor = AsignaConstructor(tipo, AppGlobalConfig.CapitulosPorPdf, NovelaActual.CarpetaPath, NovelaActual.Titulo);
        }


        /// <summary>
        /// Asigna un constructor dependiendo del tipo de archivo que se desee.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        private Constructor AsignaConstructor(TiposDocumentos tipo, int capsPorPDF, string direccion, string titulo)
        {
            switch (tipo)
            {
                case TiposDocumentos.PDF:
                    return new ConstructorPDF(capsPorPDF, direccion, titulo, CapituloImpreso, DocumentoCreado);
                default:
                    throw new NotImplementedException("No se han creado constructores para otros tipos de archivos que no sean PDF.");
            }
        }


        #endregion



        #region Props publicas


        public int DocumentosCreados { get; private set; } = 0;


        #endregion


        #region Propiedades


        private string Path => NovelaActual.CarpetaPath;


        private int CapitulosPorPdf => AppGlobalConfig.CapitulosPorPdf;


        private string TituloNovela => NovelaActual.Titulo;


        private bool CreaPdf => NovelaActual.CantidadCapitulosPorImprimir >= CapitulosPorPdf;

        public string Nombre => "Empaquetador";


        #endregion


        #region Fields


        /// <summary>
        /// Constructor que se está usando para crear los documentos.
        /// </summary>
        private readonly Constructor Constructor;


        /// <summary>
        /// Novela que se está empaquetando.
        /// </summary>
        private readonly Novela NovelaActual;


        #endregion




        #region Metodos Publicos

        /// <summary>
        /// Notifica que la novela ha finalizado.
        /// </summary>
        public void FinalizoNovela()
        {
            if (NovelaActual.TengoCapitulosPorImprimir) EmpaquetaNovela();
        }


        public void AgregaCapitulo(Capitulo CapituloNuevo)
        {
            NovelaActual.AgregaCapitulo(CapituloNuevo);
            //if (CreaPdf) legacy_EmpaquetaNovela();
            if (CreaPdf) EmpaquetaNovela();
        }

        
        #endregion


        #region Core

        private void EmpaquetaNovela()
        {
            List<Capitulo> CapitulosAImprimir = new List<Capitulo>(NovelaActual.CapitulosSinImprimir);

            Constructor.ConstruyeDocumento(CapitulosAImprimir);
        }

        #endregion


        #region Metodos para el constructor de documentos

        /// <summary>
        /// El constructor llama este metodo para notificar que un capitulo fue colocado en el documento.
        /// </summary>
        /// <param name="capitulo"></param>
        private void CapituloImpreso(Capitulo capitulo)
        {
            NovelaActual.CapituloFueImpreso(capitulo);
        }


        /// <summary>
        /// El constructor llama este metodo para notificar que un documento fue creado.
        /// </summary>
        /// <param name="tituloDocumento"></param>
        private void DocumentoCreado(string tituloDocumento)
        {
            DocumentosCreados++;
            AppGlobalMensajero.ReportaEspecial($"Creando {tituloDocumento}.", this);
        }

        #endregion

    }
}