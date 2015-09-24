using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    class GenericFile
    {
        private string path;

        public GenericFile(string path)
        {
            this.path = path;
        }

        public void EnsureDirectory()
        {
            System.IO.Directory.CreateDirectory(this.Directory);
        }

        public Uri Uri
        {
            get { return new Uri(path); }
        }

        public string Directory
        {
            get { return System.IO.Path.GetDirectoryName(path); }
        }

        public string Path
        {
            get { return path; }
        }

        public string Name
        {
            get { return System.IO.Path.GetFileNameWithoutExtension(path); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
