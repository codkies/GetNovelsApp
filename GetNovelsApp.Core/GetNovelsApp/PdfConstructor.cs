﻿using System.Collections.Generic;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using GetNovelsApp.Core.Utilidades;
using GetNovelsApp.Core.Modelos;

namespace GetNovelsApp.Core
{
    public class PdfConstructor
    {
        #region Constructores

        public PdfConstructor(Novela Novela, Configuracion Configuracion)
        {
            this.NovelaActual = Novela;
            this.ConfiguracionActual = Configuracion;
        }

        #endregion

        #region Propiedades

        public int DocumentosCreados { get; private set; } = 0;

        private string Path => ConfiguracionActual.PathCarpeta;

        private int CapitulosPorPdf => ConfiguracionActual.CapitulosPorPdf;

        private string TituloNovela => NovelaActual.Titulo;

        private bool CreaPdf => Capitulos?.Count >= CapitulosPorPdf;

        private List<Capitulo> Capitulos => NovelaActual?.CapitulosEnNovela;

        private bool QuedanCapitulosPorImprimir => NovelaActual != null;


        #endregion

        #region Fields

        private Novela NovelaActual;

        private Configuracion ConfiguracionActual;


        #endregion


        #region Methods


        //Eventos
        public void FinalizoNovela()
        {
            if (QuedanCapitulosPorImprimir) ConstruyePDF();
        }
        //-------------


        public void AgregaCapitulo(Capitulo CapituloNuevo)
        {
            NovelaActual.AgregaCapitulo(CapituloNuevo);
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
                    Mensajero.MuestraNotificacion($"Constructor-- > Creando {TituloPDF}.");

                    writer = new PdfWriter(TituloPDF);
                    pdf = new PdfDocument(writer);
                    document = new Document(pdf);
                    DocumentosCreados++;

                    capitulosEnPdf = 0;
                }

                Capitulo capitulo = Capitulos[i];                

                Paragraph header = new Paragraph($"{TituloNovela} - {capitulo.TituloCapitulo}").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);
                document.Add(header);

                LineSeparator ls = new LineSeparator(new SolidLine());
                document.Add(ls);

                Paragraph texto = new Paragraph(capitulo.Texto).SetTextAlignment(TextAlignment.JUSTIFIED).SetFontSize(15);
                document.Add(texto);

                capitulosEnPdf++;
            }

            CierraDocumento(pdf, document);
            NovelaActual = null;
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
    }
}