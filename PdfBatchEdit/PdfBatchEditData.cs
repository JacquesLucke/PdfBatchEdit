using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    class PdfBatchEditData
    {
        private SourceFiles sourceFiles = new SourceFiles();

        public PdfBatchEditData() { }

        public SourceFiles SourceFiles
        {
            get { return sourceFiles; }
        }

    }
}
