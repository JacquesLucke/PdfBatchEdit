using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    class BatchFile
    {
        private SourceFile source;
        private TargetFile target;

        public BatchFile() { }

        public static BatchFile FromPath(string path)
        {
            BatchFile file = new BatchFile();
            file.source = new SourceFile(path);
            return file;
        }

        public override string ToString()
        {
            return source.Name;
        }
    }
}
