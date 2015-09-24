using PdfSharp.Pdf;

namespace PdfBatchEdit
{
    interface IPdfEffect
    {
        void ApplyEffect(PdfDocument document);
    }
}
