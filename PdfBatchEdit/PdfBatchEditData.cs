using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    class PdfBatchEditData
    {
        private FilesBatch sourceFiles = new FilesBatch();

        public PdfBatchEditData() { }

        public FilesBatch SourceFiles
        {
            get { return sourceFiles; }
        }

    }
}
