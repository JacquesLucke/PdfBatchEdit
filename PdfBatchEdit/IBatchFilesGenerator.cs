using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    interface IBatchFilesGenerator
    {
        List<BatchFile> LoadBatchFiles();
    }
}
