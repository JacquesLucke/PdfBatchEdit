using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace PdfBatchEdit
{
    class SourceFile : GenericFile
    {
        public SourceFile(string path) : base(path) { }

        public PdfDocument Load()
        {
            return PdfReader.Open(Path, PdfDocumentOpenMode.Modify);
        }
    }
}
