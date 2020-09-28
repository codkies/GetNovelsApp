using System;
using System.Collections.Generic;

using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.GetNovelsApp;
using GetNovelsApp.Core.Empaquetador;
using GetNovelsApp.Core.Conexiones.DB;

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

        #endregion


        #region Props publicas


        public int DocumentosCreados { get; private set; } = 0;

        public string Nombre => "Empaquetador";

        #endregion



        #region Fields


        private readonly Archivador Archivador;

        #endregion




        #region Metodos Publicos
         

        public void EmpaquetaCapitulo(List<Capitulo> CapitulosDescargados, NovelaRuntimeModel novela)
        {          
            foreach (Capitulo c in CapitulosDescargados)
            {
                novela.CapituloFueDescargado(c);                
            }

            List<Capitulo> _ = Archivador.GuardaCapitulos(CapitulosDescargados, novela.ID);
        }

        
        #endregion


        #region Imprimiendo novelas.

        private void ImprimeNovela(NovelaRuntimeModel novela, TiposDocumentos tipo)
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
        private void CapituloImpreso(Capitulo capitulo, NovelaRuntimeModel novela)
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