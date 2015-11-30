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
            PdfDocument document = null;
            try
            {
                return PdfReader.Open(Path, PdfDocumentOpenMode.Modify);
            }
            catch (PdfReaderException)
            {
                FileStream fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
                int len = (int)fs.Length;
                byte[] fileArray = new byte[len];
                fs.Read(fileArray, 0, len);
                fs.Close();

                MemoryStream sourceStream = new MemoryStream(fileArray);
                sourceStream.Position = 0;
                MemoryStream outputStream = new MemoryStream();
                iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(sourceStream);
                iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(reader, outputStream);
                pdfStamper.FormFlattening = true;
                pdfStamper.Writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_4);
                pdfStamper.Writer.CloseStream = false;
                pdfStamper.Close();

                document = PdfReader.Open(outputStream, PdfDocumentOpenMode.Modify);
            }
            return document;
        }
    }
}
