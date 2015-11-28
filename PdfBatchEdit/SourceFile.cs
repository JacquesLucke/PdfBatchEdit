using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfBatchEdit
{
    public class SourceFile : GenericFile
    {
        public SourceFile(string path) : base(path) { }

        public PdfDocument Load()
        {
            return PdfReader.Open(Path, PdfDocumentOpenMode.Modify);
        }
    }
}
