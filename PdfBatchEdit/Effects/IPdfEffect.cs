using PdfSharp.Pdf;

namespace PdfBatchEdit.Effects
{
    interface IPdfEffect
    {
        void ApplyEffect(IPdfEffectLocalSettings localData, PdfDocument document);

        IPdfEffectLocalSettings GetLocalSettings();
    }

    interface IPdfEffectLocalSettings
    {
        IPdfEffect GetMainEffect();
    }
}
