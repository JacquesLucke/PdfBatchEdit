using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PdfBatchEdit
{
    class AddTextEffect : IPdfEffect
    {
        public string text = "Hello World";

        public void ApplyEffect(PdfDocument document)
        {
            PdfPage page = document.Pages[0];
            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
            XFont font = new XFont("Verdana", 15, XFontStyle.Regular);
            gfx.DrawString(text, font, XBrushes.Red, new XPoint(300, 200));
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public override string ToString()
        {
            return "Add Text: " + text;
        }
    }
}
