using System.Collections.Generic;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

using GetNovelsApp.Modelos;
using GetNovelsApp.Utilidades;

namespace GetNovelsApp
{
    public class PdfConstructor
    {
        #region Propiedades

        public int DocumentosCreados { get; private set; } = 0;

        public string Path => Configuracion.PathCarpeta;

        private int CapitulosPorPdf => Configuracion.CapitulosPorPdf;

        private string TituloNovela => Novela.Titulo;

        private bool CreaPdf => Capitulos.Count >= CapitulosPorPdf;

        #endregion

        #region Fields

        Novela Novela;

        Configuracion Configuracion;

        private List<Capitulo> Capitulos = new List<Capitulo>();

        #endregion

        //Eventos
        public void FinalizoNovela()
        {
            if (Capitulos.Count > 0) ConstruyePDF();
        }
        //-------------

        public void InicializaConstructor(Novela Novela, Configuracion Configuracion)
        {
            this.Novela = Novela;
            this.Configuracion = Configuracion;
        }

        public void AgregaCapitulo(Capitulo CapituloNuevo)
        {
            Capitulos.Add(CapituloNuevo);
            if (CreaPdf) ConstruyePDF();
        }

        private void ConstruyePDF()
        {
            int capitulosEnPdf = 0;
            int inicial = Capitulos[0].NumeroCapitulo;
            int final = Capitulos[Capitulos.Count - 1].NumeroCapitulo;

            string TituloPDF = $"{Path}{TituloNovela} - {inicial}-{final}.pdf";

            PdfWriter writer = new PdfWriter(TituloPDF);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf);
            Mensajero.MuestraNotificacion($"Constructor--> Creando {TituloPDF}.");
            DocumentosCreados++;

            for (int i = 0; i < Capitulos.Count; i++)
            {
                if (capitulosEnPdf >= CapitulosPorPdf)
                {
                    CierraDocumento(pdf, document);

                    TituloPDF = $"{Path}{TituloNovela} - {i + 1}-{i + CapitulosPorPdf}.pdf";

                    writer = new PdfWriter(TituloPDF);
                    pdf = new PdfDocument(writer);
                    document = new Document(pdf);
                    Mensajero.MuestraNotificacion($"Constructor-- > Creando { TituloPDF}.");
                    DocumentosCreados++;

                    capitulosEnPdf = 0;
                }

                Capitulo capitulo = Capitulos[i];
                string TextoCapitulo = Capitulos[i].Texto;

                Paragraph header = new Paragraph($"{TituloNovela} - Chapter {capitulo.NumeroCapitulo}").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);
                document.Add(header);

                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                Paragraph texto = new Paragraph(TextoCapitulo).SetTextAlignment(TextAlignment.JUSTIFIED).SetFontSize(15);
                document.Add(texto);

                capitulosEnPdf++;
            }

            CierraDocumento(pdf, document);
            LimpiaCapitulos();
        }


        /// <summary>
        /// Cierra el pdf y le coloca los numeros de pagina
        /// </summary>
        /// <param name="pdf"></param>
        /// <param name="document"></param>
        private static void CierraDocumento(PdfDocument pdf, Document document)
        {
            //Coloca numeros de paginas.
            //int numOfPages = pdf.GetNumberOfPages();
            //for (int x = 1; x <= numOfPages; x++)
            //{
            //    document.ShowTextAligned(
            //        new Paragraph(String.Format(x + " / " + numOfPages)),
            //        579,
            //        826,
            //        x,
            //        TextAlignment.RIGHT,
            //        VerticalAlignment.TOP,
            //        0);
            //}

            //Cierralo.
            document.Close();
        }


        /// <summary>
        /// Limpia el record de capitulos en este constructor.
        /// </summary>
        private void LimpiaCapitulos()
        {
            Capitulos.Clear();
        }
    }
}