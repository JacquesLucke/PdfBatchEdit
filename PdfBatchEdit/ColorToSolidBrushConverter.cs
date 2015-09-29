using PdfSharp.Drawing;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PdfBatchEdit
{
    [ValueConversion(typeof(XColor), typeof(SolidColorBrush))]
    class ColorToSolidBrushConverter : IValueConverter
    {
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = (SolidColorBrush)value;
            XColor xColor = new XColor();
            xColor.R = brush.Color.R;
            xColor.G = brush.Color.G;
            xColor.B = brush.Color.B;
            xColor.A = (double)brush.Color.A / 255.0;
            return xColor;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            XColor xColor = (XColor)value;
            Color color = new Color();
            color.R = xColor.R;
            color.G = xColor.G;
            color.B = xColor.B;
            color.A = System.Convert.ToByte(xColor.A * 255);
            return new SolidColorBrush(color);
        }
    }
}
