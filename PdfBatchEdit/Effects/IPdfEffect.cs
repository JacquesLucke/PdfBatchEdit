using PdfSharp.Pdf;

namespace PdfBatchEdit.Effects
{
    public interface IPdfEffect
    {
        void ApplyEffect(ILocalPdfEffectSettings localData, PdfDocument document);

        ILocalPdfEffectSettings GetLocalSettings();
    }

    public interface ILocalPdfEffectSettings
    {
        IPdfEffect GetMainEffect();
    }
}
