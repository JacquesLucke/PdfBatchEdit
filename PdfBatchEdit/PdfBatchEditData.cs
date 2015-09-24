using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    class PdfBatchEditData
    {
        private BatchFiles batchFiles = new BatchFiles();
        private PdfEffects effects = new PdfEffects();

        public PdfBatchEditData()
        {
            effects.Add(new AddTextEffect());
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
