using System.IO;

namespace PdfBatchEdit
{
    class PdfBatchEditData
    {
        private BatchFiles batchFiles = new BatchFiles();
        private PdfEffects effects = new PdfEffects();

        public PdfBatchEditData()
        {
        }

        public BatchFiles BatchFiles
        {
            get { return batchFiles; }
        }

        public PdfEffects Effects
        {
            get { return effects; }
        }

        public void Reset()
        {
            batchFiles.Clear();
        }

        public void RemoveTemporaryFiles()
        {
            RemovePreviewFiles();
        }

        private void RemovePreviewFiles()
        {
            string directoryPath = Utils.MainDirectory + "previews\\";
            foreach (string path in Directory.GetFiles(directoryPath))
            {
                try { File.Delete(path); }
                catch { }   
            }
        }
    }
}
