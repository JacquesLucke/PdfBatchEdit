using PdfSharp.Drawing;
using PdfSharp.Pdf;
using System.Collections.Generic;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PdfBatchEdit.Effects
{
    public class TextEffect : IPdfEffect, INotifyPropertyChanged
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
        private bool useOrientation = false;
        private List<LocalTextEffectSettings> localSettingsObjects = new List<LocalTextEffectSettings>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public TextEffect(string text)
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
                foreach (LocalTextEffectSettings settings in localSettingsObjects)
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

        public bool UseOrientation
        {
            get { return useOrientation; }
            set
            {
                useOrientation = value;
                NotifyPropertyChanged();
            }
        }

        public ILocalPdfEffectSettings GetLocalSettings()
        {
            LocalTextEffectSettings settings = new LocalTextEffectSettings(this);
            this.localSettingsObjects.Add(settings);
            return settings;
        }

        public bool CheckIfDocumentWillBeModified(ILocalPdfEffectSettings localDataObject)
        {
            LocalTextEffectSettings localData = (LocalTextEffectSettings)localDataObject;
            if (localData.LocalTextIsUsed && localData.Text.Trim() == "") return false;
            if (!localData.LocalTextIsUsed && this.Text.Trim() == "") return false;
            return true;
        }

        public void ApplyEffect(ILocalPdfEffectSettings localDataObject, PdfDocument document)
        {
            LocalTextEffectSettings localData = (LocalTextEffectSettings)localDataObject;
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
            XSolidBrush brush = new XSolidBrush(fontColor);

            XPoint position = CalculatePosition(page, gfx, font, drawText, relativeX, relativeY, horizontalAlignment, verticalAlignment);

            if (ShouldHandleOrientation(page)) gfx.RotateAtTransform(90, position);
            gfx.DrawString(drawText, font, brush, position);
            gfx.Dispose();
        }

        private XPoint CalculatePosition(PdfPage page, XGraphics gfx, XFont font, string text, double relativeX, double relativeY, HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            XSize size = gfx.MeasureString(text, font);

            if (!ShouldHandleOrientation(page))
            {
                XPoint position = new XPoint();
                position.X = page.Width * relativeX;
                position.Y = page.Height * relativeY;

                if (horizontal == HorizontalAlignment.Center) position.X -= size.Width / 2;
                if (horizontal == HorizontalAlignment.Right) position.X -= size.Width;
                if (vertical == VerticalAlignment.Center) position.Y += size.Height / 2;
                if (vertical == VerticalAlignment.Top) position.Y += size.Height;

                return position;
            }
            else
            {
                XPoint position = new XPoint();
                position.X = page.Width * (1 - relativeY);
                position.Y = page.Height * relativeX;
                
                if (horizontal == HorizontalAlignment.Center) position.Y -= size.Width / 2;
                if (horizontal == HorizontalAlignment.Right) position.Y -= size.Width;
                if (vertical == VerticalAlignment.Center) position.X -= size.Height / 2;
                if (vertical == VerticalAlignment.Top) position.X -= size.Height;

                return position;
            }
        }

        private bool ShouldHandleOrientation(PdfPage page)
        {
            return useOrientation && page.Width > page.Height;
        }
    }

    public class LocalTextEffectSettings : ILocalPdfEffectSettings, INotifyPropertyChanged
    {
        private TextEffect main;
        private string text = "";

        public event PropertyChangedEventHandler PropertyChanged;

        public LocalTextEffectSettings(TextEffect main, string text = "")
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
