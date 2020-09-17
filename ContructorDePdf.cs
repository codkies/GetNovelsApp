using System;
using System.Collections.Generic;

using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Draw;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

public class ContructorDePdf
{
    #region Propiedades

    //Informacion
    public int DocumentosCreados { get; private set; } = 0;

    //Configuracion
    public string Path { get; private set; }

    private List<string> Capitulos = new List<string>();

    private int CapitulosPorPdf;

    private string TituloNovela;

    private bool CreaPdf => Capitulos.Count >= CapitulosPorPdf;

    #endregion


    //Eventos
    public void FinalizoNovela()
    {
        if(Capitulos.Count > 0) ConstruyePDF();        
    }
    //-------------

    public void InicializaConstructor(string TituloNovela, int CapitulosPorPdf, string Path)
    {
        this.TituloNovela = TituloNovela;
        this.CapitulosPorPdf = CapitulosPorPdf;
        this.Path = Path;        
    }

    public void AgregaCapitulo(string CapituloNuevo)
    {
        Capitulos.Add(CapituloNuevo);
        if (CreaPdf) ConstruyePDF();
        
    }

    private void ConstruyePDF()
    {
        int capitulosEnPdf = 0;
        int numeroCap = capitulosEnPdf < 1 ? 1 : capitulosEnPdf;
        string TituloPDF = $"{Path}{TituloNovela} - {numeroCap}-{CapitulosPorPdf}.pdf";

        PdfWriter writer = new PdfWriter(TituloPDF);
        PdfDocument pdf = new PdfDocument(writer);
        Document document = new Document(pdf);
        Console.WriteLine($"Constructor--> Creando {TituloPDF}. ///");
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
                Console.WriteLine($"Constructor--> Creando {TituloPDF}. ///");
                DocumentosCreados++;

                capitulosEnPdf = 0;
            }

            string Capitulo = Capitulos[i];
            Paragraph header = new Paragraph($"{TituloNovela} - Chapter {i + 1}").SetTextAlignment(TextAlignment.CENTER).SetFontSize(20);
            document.Add(header);

            LineSeparator ls = new LineSeparator(new SolidLine());
            document.Add(ls);

            Paragraph texto = new Paragraph(Capitulo).SetTextAlignment(TextAlignment.JUSTIFIED).SetFontSize(15);
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
        int numOfPages = pdf.GetNumberOfPages();
        for (int x = 1; x <= numOfPages; x++)
        {
            document.ShowTextAligned(
                new Paragraph(String.Format("page" + x + " of " + numOfPages)),
                559,
                806,
                x,
                TextAlignment.RIGHT,
                VerticalAlignment.TOP,
                0);
        }

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

