using PdfSharp.Pdf;

namespace PdfBatchEdit
{
    class BatchFile
    {
        private SourceFile source;
        private LocalSettingsCollection localSettings;

        public BatchFile(string path)
        {
            source = new SourceFile(path);
            localSettings = new LocalSettingsCollection();
        }

        public SourceFile Source
        {
            get { return source; }
        }

        public LocalSettingsCollection LocalEffectSettings
        {
            get { return localSettings; }
        }

        public override string ToString()
        {
            return source.Name;
        }

        public GenericFile GeneratePreview(PdfEffects effects)
        {
            GenericFile file = getNewPreviewFile();
            file.EnsureDirectory();
            PdfDocument document = ApplyEffects(effects);
            document.Save(file.Path);
            return file;
        }

        private GenericFile getNewPreviewFile()
        {
            string fileName = Utils.GetRandomString(10) + ".pdf";
            string path = Utils.MainDirectory + "previews\\" + fileName;
            return new GenericFile(path);
        }

        public PdfDocument ApplyEffects(PdfEffects effects)
        {
            PdfDocument document = source.Load();
            foreach (IPdfEffectLocalSettings effect in localSettings)
            {
                effect.GetMainEffect().ApplyEffect(effect, document);
            }
            return document;
        }
    }
}
