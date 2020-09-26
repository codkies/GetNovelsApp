using GetNovelsApp.Core.Modelos;

using System.Linq;
using System.Collections.Generic;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace GetNovelsApp.Core.Empaquetadores.CreadorDocumentos.Constructores
{
    public class ConstructorPDF : ConstructorBasico
    {
        #region Constructor & Setup
        public ConstructorPDF(Novela novela, int capitulosPorPDF, string direccionGuardarPDF, string tituloHeader, NotificaCapituloImpreso capituloImpreso, NotificaDocumentoCreado documentoCreado)
        {
            this.Novela = novela;
            this.capitulosPorPDF = capitulosPorPDF;
            this.direccionGuardarPDF = direccionGuardarPDF;
            this.tituloHeader = tituloHeader;
            this.capituloImpreso = capituloImpreso;
            this.documentoCreado = documentoCreado;
        }

        readonly Novela Novela;
        readonly int capitulosPorPDF;
        readonly string direccionGuardarPDF;
        readonly string tituloHeader;
        readonly NotificaCapituloImpreso capituloImpreso;
        readonly NotificaDocumentoCreado documentoCreado;

        #endregion


        #region Propiedades para el funcionamiento interno de la clase

        protected override int CapitulosPorDoc => capitulosPorPDF;

        protected override string Path => direccionGuardarPDF;

        protected override string TituloNovela => tituloHeader;

        protected override NotificaCapituloImpreso CapituloImpreso => capituloImpreso;

        protected override NotificaDocumentoCreado DocumentoCreado => documentoCreado;

        #endregion


        #region Publico


        /// <summary>
        /// Construye un documento con los capitulos proporcionados.
        /// </summary>
        /// <param name="CapitulosAImprimir"></param>
        public override void ConstruyeDocumento(List<Capitulo> CapitulosAImprimir)
        {
            int capitulosEnPdf = 0;

            float primerCap = CapitulosAImprimir.First().NumeroCapitulo;
            float ultimoCap = CapitulosAImprimir.Last().NumeroCapitulo;

            string TituloPDF = $"{Path}{TituloNovela} - {primerCap}-{ultimoCap}.pdf";
            PdfWriter writer = new PdfWriter(TituloPDF);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            DocumentoCreado?.Invoke(TituloPDF);

            for (int i = 0; i < CapitulosAImprimir.Count; i++)
            {
                if (capitulosEnPdf >= CapitulosPorDoc)
                {
                    document.Close();

                    TituloPDF = $"{Path}{TituloNovela} - {i + 1}-{i + CapitulosPorDoc}.pdf";
                    writer = new PdfWriter(TituloPDF);
                    pdf = new PdfDocument(writer);
                    document = new Document(pdf);
                    DocumentoCreado?.Invoke(TituloPDF);

                    capitulosEnPdf = 0;
                }

                Capitulo capitulo = CapitulosAImprimir[i];

                Paragraph header = new Paragraph($"{TituloNovela} - {capitulo.TituloCapitulo}").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);
                document.Add(header);

                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                Paragraph texto = new Paragraph(capitulo.Texto).SetTextAlignment(TextAlignment.JUSTIFIED).SetFontSize(15);
                document.Add(texto);

                CapituloImpreso?.Invoke(capitulo, Novela);
                capitulosEnPdf++;
            }

            document.Close();
        }

        #endregion
    }
}
