using System.Collections.ObjectModel;
using System.IO;

namespace PdfBatchEdit
{
    public class BatchFiles : ObservableCollection<BatchFile>
    {
        public void Export(string targetDirectory)
        {
            foreach (BatchFile file in this)
            {
                string fileName = file.OutputNamePrefix + file.Source.NameWithExtension;
                string targetPath = Path.Combine(targetDirectory, fileName);
                file.Save(new GenericFile(targetPath));
            }
        }
    }
}
