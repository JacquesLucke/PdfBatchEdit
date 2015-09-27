using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System;

namespace PdfBatchEdit
{
    class AddTextEffect : IPdfEffect
    {
        private AddTextEffectGlobalSettings globalData = new AddTextEffectGlobalSettings();

        public AddTextEffect(string text)
        {
            globalData.Text = text;
        }

        public AddTextEffectGlobalSettings GlobalData
        {
            get { return globalData; }
        }

        public IPdfEffectLocalSettings GetLocalSettings()
        {
            return new AddTextEffectLocalSettings(this);
        }
        
        public void ApplyEffect(IPdfEffectLocalSettings localDataObject, PdfDocument document)
        {
            AddTextEffectLocalSettings localData = (AddTextEffectLocalSettings)localDataObject;
            string text = globalData.Text;

            PdfPage page = document.Pages[0];
            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
            XFont font = new XFont("Verdana", 15, XFontStyle.Regular);
            gfx.DrawString(text, font, XBrushes.Red, new XPoint(300, 200));
            gfx.Dispose();
        }
    }

    class AddTextEffectLocalSettings : IPdfEffectLocalSettings
    {
        private AddTextEffect main;

        public AddTextEffectLocalSettings(AddTextEffect main)
        {
            this.main = main;
        }

        public IPdfEffect GetMainEffect()
        {
            return main;
        }
    }

    class AddTextEffectGlobalSettings
    {
        private string text = "";
        private XBrush brush = XBrushes.Red;
        private XPoint position = new XPoint(300, 200);

        public AddTextEffectGlobalSettings() { }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public XBrush Brush
        {
            get { return brush; }
            set { brush = value; }
        }

        public XPoint Position
        {
            get { return position; }
            set { position = value; }
        }
    }
}
