using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        }

        private void newSourceFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Pdf Files|*.pdf";
            ofd.Multiselect = true;
            if (ofd.ShowDialog() == true)
            {
                List<BatchFile> files = new List<BatchFile>();
                foreach (string path in ofd.FileNames) { files.Add(BatchFile.FromPath(path)); }
                insertBatchFilesWithDelay(files);
            }
        }

        private void previewFile_Click(object sender, RoutedEventArgs e)
        {
            BatchFile batchFile = (BatchFile)((Button)e.Source).DataContext;
            pdfViewer.Source = batchFile.Source.Uri;
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

        private void insertBatchFilesWithDelay(List<BatchFile> batchFiles)
        {
            Task.Factory.StartNew(() =>
               {
                   foreach (BatchFile file in batchFiles)
                   {
                       Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                           {
                               data.BatchFiles.Add(file);
                           }), DispatcherPriority.Background);
                       Thread.Sleep(50);
                   }
               });
        }
    }
}
