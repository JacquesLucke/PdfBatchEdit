using System.Collections.Generic;

namespace PdfBatchEdit
{
    interface IBatchFilesGenerator
    {
        List<BatchFile> LoadBatchFiles();
    }
}
