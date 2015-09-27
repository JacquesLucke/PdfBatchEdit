using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System;

namespace PdfBatchEdit
{
    class AddTextEffect : IPdfEffect
    {
        private string text = "";
        private double relativeX = 0.99;
        private double relativeY = 0.01;
        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Right;
        private VerticalAlignment verticalAlignment = VerticalAlignment.Top;
        private PagesType pages = PagesType.All;

        public AddTextEffect(string text)
        {
            this.text = text;
        }

        public string Text
        {
            get { return text; }
            set { text = value; }
        }

        public double RelativeX
        {
            get { return relativeX; }
            set { relativeX = value; }
        }

        public double RelativeY
        {
            get { return relativeY; }
            set { relativeY = value; }
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

            for (int i = 0; i < document.PageCount; i++)
            {
                PdfPage page = document.Pages[i];
                if (pages == PagesType.All) WriteOnPage(page, drawText);
                if (pages == PagesType.First && i == 0)
                {
                    WriteOnPage(page, drawText);
                    break;
                }
            }
        }

        private void WriteOnPage(PdfPage page, string drawText)
        {
            XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append);
            XFont font = new XFont("Verdana", 15, XFontStyle.Regular);

            XSize size = gfx.MeasureString(drawText, font);
            XPoint position = new XPoint();

            position.X = page.Width * relativeX;
            position.Y = page.Height * relativeY;
            position = CorrectPosition(position, size);

            gfx.DrawString(drawText, font, XBrushes.Red, position);
            gfx.Dispose();
        }

        private XPoint CorrectPosition(XPoint position, XSize size)
        {
            if (horizontalAlignment == HorizontalAlignment.Center) position.X -= size.Width / 2;
            if (horizontalAlignment == HorizontalAlignment.Right) position.X -= size.Width;
            if (verticalAlignment == VerticalAlignment.Center) position.Y += size.Height / 2;
            if (verticalAlignment == VerticalAlignment.Top) position.Y += size.Height;
            return position;
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

    public enum HorizontalAlignment
    {
        Left,
        Right,
        Center
    }

    public enum VerticalAlignment
    {
        Top,
        Bottom,
        Center
    }

    public enum PagesType
    {
        First,
        All
    }
}
