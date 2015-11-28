namespace PdfBatchEdit.Templates
{
    public interface ITemplate
    {
        void Execute(PdfBatchEditData data);
    }
}
