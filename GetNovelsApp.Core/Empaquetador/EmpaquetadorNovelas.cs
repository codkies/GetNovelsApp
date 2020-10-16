using System;
using System.Collections.Generic;

using GetNovelsApp.Core.Modelos;
using GetNovelsApp.Core.Reportaje;
using GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores;
using GetNovelsApp.Core.Empaquetador;
using GetNovelsApp.Core.Conexiones.DB;
using System.Linq;
using System.Threading.Tasks;

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


        public void EmpaquetaCapitulo(Capitulo capituloDescargado, INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, IProgress<IReporte> progreso)
        {
            novela.CapituloFueDescargado(capituloDescargado);

            Archivador.GuardaCapitulosAsync(capituloDescargado, novela.ID);

            var nuevo_Reporte = GetNovelsFactory.FabricaReporteNovela(novela.Capitulos.ToList().Count, novela.CapitulosDescargados.ToList().Count, "Descargando", this, novela.Titulo);

            progreso.Report(nuevo_Reporte);
        }


        #endregion


        #region Imprimiendo novelas.

        public void ImprimeNovela(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, TiposDocumentos tipo)
        {
            string Path = LocalPathManager.DefinePathNovela(novela);
            IConstructor Constructor = GetNovelsFactory.FabricaConstructor(novela, tipo, GetNovelsConfig.CapitulosPorPdf, Path, novela.Titulo, CapituloImpreso, DocumentoCreado);

            List<Capitulo> CapitulosAImprimir = new List<Capitulo>(novela.CapitulosDescargados);

            Constructor.ConstruyeDocumento(CapitulosAImprimir);
        }

        public async Task ImprimeNovela(INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela, TiposDocumentos tipo, IProgress<IReporte> progress)
        {
            var reporte = GetNovelsFactory.FabricaReporteNovela(novela.CapitulosDescargados.ToList().Count, 0, "Guardando PDF", this, novela.Titulo);
            progress.Report(reporte);

            string Path = LocalPathManager.DefinePathNovela(novela);
            IConstructor Constructor = GetNovelsFactory.FabricaConstructor(novela, tipo, GetNovelsConfig.CapitulosPorPdf, Path, novela.Titulo, CapituloImpreso, DocumentoCreado);

            List<Capitulo> CapitulosAImprimir = new List<Capitulo>(novela.CapitulosDescargados);

            await Task.Run( ()=> Constructor.ConstruyeDocumento(CapitulosAImprimir, progress));
        }

        #endregion


        #region Metodos para el constructor de documentos

        /// <summary>
        /// El constructor llama este metodo para notificar que un capitulo fue colocado en el documento.
        /// </summary>
        /// <param name="capitulo"></param>
        private void CapituloImpreso(Capitulo capitulo, INovela<IEnumerable<Capitulo>, IEnumerable<string>, IEnumerable<Uri>> novela)
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