using System;

namespace PdfBatchEdit
{
    public class GenericFile
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

        public bool Exists
        {
            get { return System.IO.File.Exists(path); }
        }

        public string NameWithExtension
        {
            get { return System.IO.Path.GetFileName(path); }
        }

        public string Extension
        {
            get { return System.IO.Path.GetExtension(path); }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
