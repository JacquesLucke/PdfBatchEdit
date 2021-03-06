﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PdfBatchEdit
{
    public partial class ColorPickerDialog : Window
    {
        public ColorPickerDialog()
        {
            InitializeComponent();
        }

        private void okButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        public Color SelectedColor
        {
            get
            {
                Color color = new Color();
                color.R = colorCanvas.R;
                color.G = colorCanvas.G;
                color.B = colorCanvas.B;
                color.A = colorCanvas.A;
                return color;
            }
            set
            {
                colorCanvas.R = value.R;
                colorCanvas.G = value.G;
                colorCanvas.B = value.B;
                colorCanvas.A = value.A;
            }
        }
    }
}
