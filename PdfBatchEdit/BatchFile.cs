using PdfBatchEdit.Effects;
using PdfSharp.Pdf;
using System;

namespace PdfBatchEdit
{
    public class BatchFile
    {
        private SourceFile source;
        private LocalSettingsCollection localSettings;
        private string outputNamePrefix = "";
        private FileNameType nameType = FileNameType.SourceNameAndPrefix;

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

        public FileNameType OutputNameType
        {
            get { return nameType; }
            set { nameType = value; }
        }

        public string OutputNamePrefix
        {
            get { return outputNamePrefix; }
            set { outputNamePrefix = value; }
        }

        public string OutputFileName
        {
            get
            {
                if (nameType == FileNameType.SourceName) return source.NameWithExtension;
                if (nameType == FileNameType.SourceNameAndPrefix) return outputNamePrefix + source.NameWithExtension;
                if (nameType == FileNameType.PrefixOnly) return outputNamePrefix + source.Extension;
                throw new Exception("Execution should not reach this point.");
            }
        }

        public ILocalPdfEffectSettings GetLocalSettingsForEffect(IPdfEffect effect)
        {
            foreach(ILocalPdfEffectSettings settings in localSettings)
            {
                if (settings.GetMainEffect() == effect) return settings;
            }
            return null;
        }

        public GenericFile GeneratePreview()
        {
            GenericFile file = getNewPreviewFile();
            Save(file);
            return file;
        }

        public void Save(GenericFile file, bool overwrite = true, bool overwriteSource = false)
        {
            if (file.Exists && !overwrite) throw new Exception("You are not allowed to overwrite existing files."); ;
            if (file.Path == source.Path && !overwriteSource) throw new Exception("You are not allowed to overwrite the source file.");

            file.EnsureDirectory();
            PdfDocument document = ApplyEffects();
            document.Save(file.Path);
        }

        private GenericFile getNewPreviewFile()
        {
            string fileName = Utils.GetRandomString(10) + ".pdf";
            string path = Utils.MainDirectory + "previews\\" + fileName;
            return new GenericFile(path);
        }

        public PdfDocument ApplyEffects()
        {
            PdfDocument document = source.Load();
            foreach (ILocalPdfEffectSettings effect in localSettings)
            {
                effect.GetMainEffect().ApplyEffect(effect, document);
            }
            return document;
        }
    }

    public enum FileNameType
    {
        SourceName,
        SourceNameAndPrefix,
        PrefixOnly
    }
}
