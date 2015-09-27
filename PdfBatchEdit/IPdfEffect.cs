using PdfSharp.Pdf;

namespace PdfBatchEdit
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
