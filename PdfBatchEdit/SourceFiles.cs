using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    class SourceFiles : IEnumerable
    {
        private List<SourceFile> files = new List<SourceFile>();

        public void New(string path)
        {
            SourceFile file = new SourceFile(path);
            files.Add(file);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new SourceFilesEnum(files);
        }
    }

    class SourceFilesEnum : IEnumerator
    {
        private List<SourceFile> files;
        private int position = -1;

        public SourceFilesEnum(List<SourceFile> files)
        {
            this.files = files;
        }

        public object Current
        {
            get
            {
                return GetCurrent();
            }
        }

        private SourceFile GetCurrent()
        {
            try
            {
                return files[position];
            }
            catch (IndexOutOfRangeException)
            {
                throw new InvalidOperationException();
            }
        }

        public bool MoveNext()
        {
            position++;
            return (position < files.Count);
        }

        public void Reset()
        {
            position = -1;
        }
    }
}
