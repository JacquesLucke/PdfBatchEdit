using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    class FilesBatch : IEnumerable
    {
        private List<BatchFile> files = new List<BatchFile>();

        public void New(string path)
        {
            BatchFile file = BatchFile.FromPath(path);
            files.Add(file);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)new FilesBatchEnum(files);
        }
    }

    class FilesBatchEnum : IEnumerator
    {
        private List<BatchFile> files;
        private int position = -1;

        public FilesBatchEnum(List<BatchFile> files)
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

        private BatchFile GetCurrent()
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
