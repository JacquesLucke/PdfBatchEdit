using System.Collections.Generic;

namespace PdfBatchEdit
{
    public interface IBatchFilesGenerator
    {
        List<BatchFile> LoadBatchFiles();
    }
}
