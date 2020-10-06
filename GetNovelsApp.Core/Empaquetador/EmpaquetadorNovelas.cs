using System;
using System.Collections.Generic;

using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
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
            GetNovelsEvents.ImprimeNovela += ImprimeNovela;
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

        public void EscuchaNovela(NovelaRT novela)
        {

        }


        public void EmpaquetaCapitulo(List<Capitulo> CapitulosDescargados, INovela novela, IProgress<IReporteNovela> progreso)
        {
            foreach (Capitulo c in CapitulosDescargados)
            {
                novela.CapituloFueDescargado(c);
            }

            Archivador.GuardaCapitulosAsync(CapitulosDescargados, novela.ID);

            var nuevo_Reporte = GetNovelsFactory.FabricaReporteNovela(novela.Capitulos.Count, novela.CapitulosDescargados.Count, novela.ID, this);

            progreso.Report(nuevo_Reporte);
           
        }

        public void EmpaquetaCapitulo(Capitulo capituloDescargado, INovela novela, IProgress<IReporteNovela> progreso)
        {
            novela.CapituloFueDescargado(capituloDescargado);

            Archivador.GuardaCapitulosAsync(capituloDescargado, novela.ID);

            var nuevo_Reporte = GetNovelsFactory.FabricaReporteNovela(novela.Capitulos.Count, novela.CapitulosDescargados.Count, novela.ID, this);

            progreso.Report(nuevo_Reporte);
        }


        #endregion


        #region Imprimiendo novelas.

        private void ImprimeNovela(INovela novela, TiposDocumentos tipo)
        {
            string Path = LocalPathManager.DefinePathNovela(novela);
            IConstructor Constructor = GetNovelsFactory.FabricaConstructor(novela, tipo, GetNovelsConfig.CapitulosPorPdf, Path, novela.Titulo, CapituloImpreso, DocumentoCreado);

            List<Capitulo> CapitulosAImprimir = new List<Capitulo>(novela.CapitulosDescargados);

            Constructor.ConstruyeDocumento(CapitulosAImprimir);
        }

        #endregion


        #region Metodos para el constructor de documentos

        /// <summary>
        /// El constructor llama este metodo para notificar que un capitulo fue colocado en el documento.
        /// </summary>
        /// <param name="capitulo"></param>
        private void CapituloImpreso(Capitulo capitulo, INovela novela)
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
            GetNovelsComunicador.ReportaEspecial($"Creando {tituloDocumento}.", this);
        }

        #endregion

    }
}