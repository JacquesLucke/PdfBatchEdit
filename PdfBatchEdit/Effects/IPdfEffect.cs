using PdfSharp.Pdf;

namespace PdfBatchEdit.Effects
{
    public interface IPdfEffect
    {
        void ApplyEffect(ILocalPdfEffectSettings localData, PdfDocument document);

        bool CheckIfDocumentWillBeModified(ILocalPdfEffectSettings localData);

        ILocalPdfEffectSettings GetLocalSettings();
    }

    public interface ILocalPdfEffectSettings
    {
        IPdfEffect GetMainEffect();
    }
}
