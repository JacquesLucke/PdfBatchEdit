using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Forms = System.Windows.Forms;

namespace PdfBatchEdit
{
    public partial class MainWindow : Window
    {
        PdfBatchEditData data;

        public MainWindow()
        {
            InitializeComponent();
            data = new PdfBatchEditData();
            filesListBox.DataContext = data.BatchFiles;
            effectsListBox.DataContext = data.Effects;
            data.AddEffectToAllFiles(new AddTextEffect("Hello World"));
        }

        private void newSourceFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pdf Files|*.pdf";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                insertFiles(ofd.FileNames);
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Do you really want to reset the program?", "Confirm Dialog", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                data.Reset();
                UpdatePreviewFromSelection();
            }
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            if (filesListBox.SelectedItem == null) return;

            if (MessageBox.Show("Do you really want to remove the selected file?", "Confirm Dialog", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                BatchFile fileToRemove = (BatchFile)filesListBox.SelectedItem;
                data.BatchFiles.Remove(fileToRemove);
                UpdatePreviewFromSelection();
            }
        }

        private void insertFiles(string[] paths)
        {
            foreach (string path in paths)
            {
                data.AddFileWithAllEffects(path);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            data.RemoveTemporaryFiles();
        }

        private void exportButton_Click(object sender, RoutedEventArgs e)
        {
            if (data.BatchFiles.Count == 0) return;

            Forms.FolderBrowserDialog fbd = new Forms.FolderBrowserDialog();
            if (fbd.ShowDialog() == Forms.DialogResult.OK)
            {
                try
                {
                    data.BatchFiles.Export(fbd.SelectedPath);
                    if (MessageBox.Show("All files saved. Open Windows Explorer?", "Successful Export", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        System.Diagnostics.Process.Start(fbd.SelectedPath);
                    }
                }
                catch (Exception exception)
                {
                    MessageBox.Show(exception.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Stop);
                }
                
            }
        }

        private void chooseColorButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            ColorPickerDialog cpd = new ColorPickerDialog();
            cpd.SelectedColor = ((SolidColorBrush)button.Background).Color;
            if (cpd.ShowDialog() == true)
            {
                button.Background = new SolidColorBrush(cpd.SelectedColor);
            }
        }

        private void filesListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdatePreviewFromSelection();
        }

        private void refreshPreviewButton_Click(object sender, RoutedEventArgs e)
        {
            UpdatePreviewFromSelection();
        }

        private void UpdatePreviewFromSelection()
        {
            if (filesListBox.SelectedItem == null)
            {
                pdfViewer.Source = null;
                return;
            }

            BatchFile batchFile = (BatchFile)filesListBox.SelectedItem;
            GenericFile previewFile = batchFile.GeneratePreview();
            pdfViewer.Source = previewFile.Uri;
        }
    }
}
