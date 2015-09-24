using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace PdfBatchEdit
{
    class AddTextEffect : IPdfEffect
    {
        public string Text = "Hello World";

        public void ApplyEffect(PdfDocument document)
        {
            PdfPage page = document.Pages[0];
            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
            XFont font = new XFont("Verdana", 15, XFontStyle.Regular);
            gfx.DrawString(Text, font, XBrushes.Red, new XPoint(300, 200));
        }

        public override string ToString()
        {
            return "Add Text: " + Text;
        }
    }
}
