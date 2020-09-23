using System.Collections.Generic;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.Core.Modelos;
using System.Linq;

namespace GetNovelsApp.Core
{
    /// <summary>
    /// Encargado de aceptar capitulos e imprimirlos.
    /// </summary>
    public class PdfConstructor
    {
        #region Constructores

        public PdfConstructor(Novela Novela)
        {
            this.NovelaActual = Novela;
        }

        #endregion


        #region Props publicas

        public int DocumentosCreados { get; private set; } = 0;

        #endregion


        #region Propiedades


        private string Path => NovelaActual.CarpetaPath;


        private int CapitulosPorPdf => Configuracion.CapitulosPorPdf;


        private string TituloNovela => NovelaActual.Titulo;


        private bool CreaPdf => NovelaActual.CantidadCapitulosPorImprimir >= CapitulosPorPdf;
        

        private bool HayCapitulosPorImprimir => NovelaActual.TengoCapitulosPorImprimir;


        #endregion


        #region Fields

        private Novela NovelaActual;

        #endregion

         
        #region Metodos Publicos


        /// <summary>
        /// Evento?
        /// </summary>
        public void FinalizoNovela()
        {
            if (HayCapitulosPorImprimir) ConstruyePDF();
        }
        


        public void AgregaCapitulo(Capitulo CapituloNuevo)
        {            
            NovelaActual.AgregaCapitulo(CapituloNuevo);
            if (CreaPdf) ConstruyePDF();
        }

        #region Privados
        private void ConstruyePDF()
        {
            int capitulosEnPdf = 0;
            float inicial = NovelaActual.ConsigueCapituloSinImprimir(0).NumeroCapitulo;
            float final = NovelaActual.ConsigueCapituloSinImprimir(NovelaActual.CantidadCapitulosPorImprimir - 1).NumeroCapitulo;

            string TituloPDF = $"{Path}{TituloNovela} - {inicial}-{final}.pdf";

            //Esta lista se edita cuando se impriminen los caps. Hay que tomar referencia de ella antes de comenzar la iteracion.
            List<Capitulo> CapitulosAImprimir = new List<Capitulo>(NovelaActual.CapitulosSinImprimir);

            PdfWriter writer = new PdfWriter(TituloPDF);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            Mensajero.MuestraNotificacion($"Constructor--> Creando {TituloPDF}.");
            DocumentosCreados++;

            for (int i = 0; i < CapitulosAImprimir.Count; i++)
            {
                if (capitulosEnPdf >= CapitulosPorPdf)
                {
                    CierraDocumento(pdf, document);

                    TituloPDF = $"{Path}{TituloNovela} - {i + 1}-{i + CapitulosPorPdf}.pdf";
                    Mensajero.MuestraNotificacion($"Constructor-- > Creando {TituloPDF}.");

                    writer = new PdfWriter(TituloPDF);
                    pdf = new PdfDocument(writer);
                    document = new Document(pdf);
                    DocumentosCreados++;

                    capitulosEnPdf = 0;
                }

                Capitulo capitulo = CapitulosAImprimir[i];

                Paragraph header = new Paragraph($"{TituloNovela} - {capitulo.TituloCapitulo}").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);
                document.Add(header);

                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                Paragraph texto = new Paragraph(capitulo.Texto).SetTextAlignment(TextAlignment.JUSTIFIED).SetFontSize(15);
                document.Add(texto);

                NovelaActual.CapituloFueImpreso(capitulo);
                capitulosEnPdf++;
            }

            CierraDocumento(pdf, document);
        }


        /// <summary>
        /// Cierra el pdf y le coloca los numeros de pagina
        /// </summary>
        /// <param name="pdf"></param>
        /// <param name="document"></param>
        private static void CierraDocumento(PdfDocument pdf, Document document)
        {
            document.Close();
        } 

        #endregion


        #endregion
    }
}