using PdfBatchEdit.Effects;
using System.IO;

namespace PdfBatchEdit
{
    public class PdfBatchEditData
    {
        private BatchFiles batchFiles = new BatchFiles();
        private PdfEffects effects = new PdfEffects();
        private AddonManager addonManager;

        public PdfBatchEditData()
        {   
            addonManager = new AddonManager(this);
            addonManager.LoadAddons();
            addonManager.ExecuteAddons();
        }

        public void AddEffectToAllFiles(IPdfEffect effect)
        {
            effects.Add(effect);
            foreach (BatchFile file in batchFiles)
            {
                file.LocalEffectSettings.Add(effect.GetLocalSettings());
            }
        }

        public BatchFile AddFileWithAllEffects(string path)
        {
            BatchFile file = new BatchFile(path);
            batchFiles.Add(file);
            foreach (IPdfEffect effect in effects)
            {
                file.LocalEffectSettings.Add(effect.GetLocalSettings());
            }
            return file;
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
