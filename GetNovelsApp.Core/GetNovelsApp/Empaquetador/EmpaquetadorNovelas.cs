using System;
using System.Collections.Generic;

using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.GetNovelsApp.Empaquetador.BaseDatos;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Empaquetador;

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

        public EmpaquetadorNovelas(Archivador archivador)
        {
            EventsManager.ImprimeNovela += ImprimeNovela;
            Archivador = archivador;
        }


        ///// <summary>
        ///// Asigna un constructor dependiendo del tipo de archivo que se desee.
        ///// </summary>
        ///// <param name="tipo"></param>
        ///// <returns></returns>
        //private IConstructor AsignaConstructor(Novela novela, TiposDocumentos tipo, int capsPorPDF, string direccion, string titulo)
        //{
        //    switch (tipo)
        //    {
        //        case TiposDocumentos.PDF:
        //            return new ConstructorPDF(novela, capsPorPDF, direccion, titulo, CapituloImpreso, DocumentoCreado);
        //        default:
        //            throw new NotImplementedException("No se han creado constructores para otros tipos de archivos que no sean PDF.");
        //    }
        //}


        #endregion


        #region Props publicas


        public int DocumentosCreados { get; private set; } = 0;

        public string Nombre => "Empaquetador";

        #endregion



        #region Fields


        private readonly Archivador Archivador;

        #endregion




        #region Metodos Publicos
         

        public void AgregaCapitulo(Capitulo CapituloNuevo, Novela novela)
        {
            if (novela.CapitulosImpresos.Contains(CapituloNuevo))
            {
                throw new Exception("Este capitulo ya fue descargado");
            }

            novela.CapituloFueDescargado(CapituloNuevo);
            Archivador.GuardaCapituloDB(CapituloNuevo, novela.ID);
        }

        
        #endregion


        #region Imprimiendo novelas.

        private void ImprimeNovela(Novela novela, TiposDocumentos tipo)
        {
            string Path = LocalPathManager.DefinePathNovela(novela);
            IConstructor Constructor = Factory.AsignaConstructor(novela, tipo, GetNovelsConfig.CapitulosPorPdf, Path, novela.Titulo, CapituloImpreso, DocumentoCreado);

            List<Capitulo> CapitulosAImprimir = new List<Capitulo>(novela.CapitulosDescargados);

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