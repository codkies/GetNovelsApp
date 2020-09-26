using System;
using System.Collections.Generic;

using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Configuracion;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.GetNovelsApp.Empaquetador.BaseDatos;

namespace GetNovelsApp.Core.Empaquetadores
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
    public class EmpaquetadorNovelas : IReportero
    {        

        #region Constructores & setup

        public EmpaquetadorNovelas()
        {
            EventsManager.ImprimeNovela += ImprimeNovela;
            Archivador = new Archivador();
        }


        /// <summary>
        /// Asigna un constructor dependiendo del tipo de archivo que se desee.
        /// </summary>
        /// <param name="tipo"></param>
        /// <returns></returns>
        private IConstructor AsignaConstructor(Novela novela, TiposDocumentos tipo, int capsPorPDF, string direccion, string titulo)
        {
            switch (tipo)
            {
                case TiposDocumentos.PDF:
                    return new ConstructorPDF(novela, capsPorPDF, direccion, titulo, CapituloImpreso, DocumentoCreado);
                default:
                    throw new NotImplementedException("No se han creado constructores para otros tipos de archivos que no sean PDF.");
            }
        }


        #endregion


        #region Props publicas


        public int DocumentosCreados { get; private set; } = 0;

        public string Nombre => "Empaquetador";

        #endregion



        #region Fields


        /// <summary>
        /// Constructor que se está usando para crear los documentos.
        /// </summary>
        private IConstructor Constructor;


        private readonly Archivador Archivador;

        #endregion




        #region Metodos Publicos


        public void AgregaCapitulo(Capitulo CapituloNuevo, Novela novela)
        {
            if (novela.CapitulosImpresos.Contains(CapituloNuevo))
            {
                throw new Exception("Este capitulo ya fue descargado");
            }

            novela.AgregaCapitulo(CapituloNuevo);
            Archivador.CapituloObtenido(CapituloNuevo, novela);
        }

        
        #endregion


        #region Core

        private void ImprimeNovela(Novela novela, TiposDocumentos tipo)
        {
            Constructor = AsignaConstructor(novela, tipo, GetNovelsConfig.CapitulosPorPdf, novela.CarpetaPath, novela.Titulo);
            List<Capitulo> CapitulosAImprimir = new List<Capitulo>(novela.CapitulosSinImprimir);

            Constructor.ConstruyeDocumento(CapitulosAImprimir);
        }

        #endregion


        #region Metodos para el constructor de documentos

        /// <summary>
        /// El constructor llama este metodo para notificar que un capitulo fue colocado en el documento.
        /// </summary>
        /// <param name="capitulo"></param>
        private void CapituloImpreso(Capitulo capitulo, Novela novela)
        {
            novela.CapituloFueImpreso(capitulo);
        }


        /// <summary>
        /// El constructor llama este metodo para notificar que un documento fue creado.
        /// </summary>
        /// <param name="tituloDocumento"></param>
        private void DocumentoCreado(string tituloDocumento)
        {
            DocumentosCreados++;
            Comunicador.ReportaEspecial($"Creando {tituloDocumento}.", this);
        }

        #endregion

    }
}