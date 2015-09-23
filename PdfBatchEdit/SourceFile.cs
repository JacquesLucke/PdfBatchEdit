using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace PdfBatchEdit
{
    class SourceFile
    {
        private string path;

        public SourceFile(string path)
        {
            this.path = path;
        }

        public Uri Uri
        {
            get { return new Uri(path); }
        }

        public string Name
        {
            get { return Path.GetFileNameWithoutExtension(path); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
