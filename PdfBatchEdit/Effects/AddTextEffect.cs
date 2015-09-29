using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PdfBatchEdit.Effects
{
    class AddTextEffect : IPdfEffect, INotifyPropertyChanged
    {
        private string text = "";
        private bool useLocalTexts = false;
        private double relativeX = 0.5;
        private double relativeY = 0.01;
        private HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center;
        private VerticalAlignment verticalAlignment = VerticalAlignment.Top;
        private PagesType pages = PagesType.First;
        private double fontSize = 12;
        private XColor fontColor = XColors.Red;
        private List<AddTextEffectLocalSettings> localSettingsObjects = new List<AddTextEffectLocalSettings>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public AddTextEffect(string text)
        {
            this.text = text;
        }

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                NotifyPropertyChanged();
            }
        }

        public bool UseLocalTexts
        {
            get { return useLocalTexts; }
            set
            {
                useLocalTexts = value;
                NotifyPropertyChanged();
                foreach (AddTextEffectLocalSettings settings in localSettingsObjects)
                    settings.UseLocalTextChanged();
            }
        }

        public double RelativeX
        {
            get { return relativeX; }
            set
            {
                relativeX = value;
                NotifyPropertyChanged();
            }
        }

        public double RelativeY
        {
            get { return relativeY; }
            set
            {
                relativeY = value;
                NotifyPropertyChanged();
            }
        }

        public double FontSize
        {
            get { return fontSize; }
            set
            {
                fontSize = value;
                NotifyPropertyChanged();
            }
        }

        public XColor FontColor
        {
            get { return fontColor; }
            set
            {
                fontColor = value;
                NotifyPropertyChanged();
            }
        }

        public PagesType Pages
        {
            get { return pages; }
            set
            {
                pages = value;
                NotifyPropertyChanged();
            }
        }

        public HorizontalAlignment HorizontalAlignment
        {
            get { return horizontalAlignment; }
            set
            {
                horizontalAlignment = value;
                NotifyPropertyChanged();
            }
        }

        public VerticalAlignment VerticalAlignment
        {
            get { return verticalAlignment; }
            set
            {
                verticalAlignment = value;
                NotifyPropertyChanged();
            }
        }

        public IPdfEffectLocalSettings GetLocalSettings()
        {
            AddTextEffectLocalSettings settings = new AddTextEffectLocalSettings(this);
            this.localSettingsObjects.Add(settings);
            return settings;
        }

        public void ApplyEffect(IPdfEffectLocalSettings localDataObject, PdfDocument document)
        {
            AddTextEffectLocalSettings localData = (AddTextEffectLocalSettings)localDataObject;
            string drawText = Text;
            if (useLocalTexts) drawText = localData.Text;

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
            XFont font = new XFont("Verdana", fontSize, XFontStyle.Regular);

            XSize size = gfx.MeasureString(drawText, font);
            XPoint position = new XPoint();
            position.X = page.Width * relativeX;
            position.Y = page.Height * relativeY;
            position = CorrectPosition(position, size);

            XSolidBrush brush = new XSolidBrush(fontColor);

            gfx.DrawString(drawText, font, brush, position);
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

    class AddTextEffectLocalSettings : IPdfEffectLocalSettings, INotifyPropertyChanged
    {
        private AddTextEffect main;
        private string text = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public AddTextEffectLocalSettings(AddTextEffect main, string text = "")
        {
            this.main = main;
            this.text = text;
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

        public bool LocalTextIsUsed
        {
            get { return main.UseLocalTexts; }
        }

        public void UseLocalTextChanged()
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("LocalTextIsUsed"));
            }
        }
    }
}
