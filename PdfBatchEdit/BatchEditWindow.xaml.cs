using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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
            data.AddEffectToAllFiles(new AddTextEffect("Peter"));

        }

        private void newSourceFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pdf Files|*.pdf";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                insertBatchFilesWithDelay(ofd.FileNames);
            }
        }

        private void previewFile_Click(object sender, RoutedEventArgs e)
        {
            BatchFile batchFile = (BatchFile)((Button)e.Source).DataContext;
            GenericFile previewFile = batchFile.GeneratePreview(data.Effects);
            pdfViewer.Source = previewFile.Uri;
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            data.Reset();
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            List<BatchFile> itemsToRemove = new List<BatchFile>();
            foreach (BatchFile batchFile in filesListBox.SelectedItems)
            {
                itemsToRemove.Add(batchFile);
            }
            foreach (BatchFile batchFile in itemsToRemove)
            {
                data.BatchFiles.Remove(batchFile);
            }
        }

        private void insertBatchFilesWithDelay(string[] paths)
        {
            Task.Factory.StartNew(() =>
               {
                   foreach (string path in paths)
                   {
                       Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                           {
                               data.AddFileWithAllEffects(path);
                           }), DispatcherPriority.Background);
                       Thread.Sleep(50);
                   }
               });
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            data.RemoveTemporaryFiles();
        }
    }
}
