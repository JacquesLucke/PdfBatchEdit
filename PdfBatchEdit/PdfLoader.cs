using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System.IO;

namespace PdfBatchEdit
{
    public class PdfLoader
    {
        public static PdfDocument Load(string path)
        {
            try
            {
                return PdfReader.Open(path, PdfDocumentOpenMode.Modify);
            }
            catch
            {
                MemoryStream pdfStream = MemoryStreamFromPath(path);
                MemoryStream convertedPdfStream = GetStreamWithConvertedPdf(pdfStream);
                return PdfReader.Open(convertedPdfStream, PdfDocumentOpenMode.Modify);
            }
        }

        private static MemoryStream GetStreamWithConvertedPdf(MemoryStream source)
        {
            MemoryStream output = new MemoryStream();
            iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(source);
            iTextSharp.text.pdf.PdfStamper pdfStamper = new iTextSharp.text.pdf.PdfStamper(reader, output);
            pdfStamper.FormFlattening = true;
            pdfStamper.Writer.SetPdfVersion(iTextSharp.text.pdf.PdfWriter.PDF_VERSION_1_4);
            pdfStamper.Writer.CloseStream = false;
            pdfStamper.Close();

            return output;
        }

        private static MemoryStream MemoryStreamFromPath(string path)
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            int len = (int)fs.Length;
            byte[] array = new byte[len];
            fs.Read(array, 0, len);
            fs.Close();

            MemoryStream ms = new MemoryStream(array);
            ms.Position = 0;
            return ms;
        }
    }
}
