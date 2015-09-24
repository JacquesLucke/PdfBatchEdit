using System.Collections.ObjectModel;

namespace PdfBatchEdit
{
    class BatchFiles : ObservableCollection<BatchFile>
    {
        public void NewBatchFile(string path)
        {
            BatchFile file = BatchFile.FromPath(path);
            this.Add(file);
        }
    }
}
