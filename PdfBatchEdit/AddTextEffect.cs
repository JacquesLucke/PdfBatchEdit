using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System;

namespace PdfBatchEdit
{
    class AddTextEffect : IPdfEffect
    {
        private string text { get; set; }

        public AddTextEffect(string text)
        {
            this.text = text;
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public IPdfEffectLocalSettings GetLocalSettings()
        {
            return new AddTextEffectLocalSettings(this);
        }
        
        public void ApplyEffect(IPdfEffectLocalSettings localDataObject, PdfDocument document)
        {
            AddTextEffectLocalSettings localData = (AddTextEffectLocalSettings)localDataObject;
            string drawText = Text;
            if (localData.UseLocalText) drawText = localData.Text;

            PdfPage page = document.Pages[0];
            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
            XFont font = new XFont("Verdana", 15, XFontStyle.Regular);
            gfx.DrawString(drawText, font, XBrushes.Red, new XPoint(300, 200));
            gfx.Dispose();
        }
    }

    class AddTextEffectLocalSettings : IPdfEffectLocalSettings
    {
        private AddTextEffect main;
        private string text = "";
        private bool useLocalText = false;

        public AddTextEffectLocalSettings(AddTextEffect main)
        {
            this.main = main;
        }

        public IPdfEffect GetMainEffect()
        {
            return main;
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public bool UseLocalText
        {
            get { return useLocalText; }
            set { useLocalText = value; }
        }
    }
}
