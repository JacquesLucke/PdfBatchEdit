using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;

namespace PdfBatchEdit
{
    public class SourceFile : GenericFile
    {
        public SourceFile(string path) : base(path) { }

        public PdfDocument Load()
        {
            return PdfLoader.Load(Path);
        }
    }
}
