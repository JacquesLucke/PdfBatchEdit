using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
