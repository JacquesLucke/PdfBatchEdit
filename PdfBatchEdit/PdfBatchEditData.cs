using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    class PdfBatchEditData
    {
        private BatchFiles batchFiles = new BatchFiles();

        public PdfBatchEditData() { }

        public BatchFiles BatchFiles
        {
            get { return batchFiles; }
        }

    }
}
