using PdfSharp.Pdf;

namespace PdfBatchEdit.Effects
{
    interface IPdfEffect
    {
        void ApplyEffect(ILocalPdfEffectSettings localData, PdfDocument document);

        ILocalPdfEffectSettings GetLocalSettings();
    }

    interface ILocalPdfEffectSettings
    {
        IPdfEffect GetMainEffect();
    }
}
