using PdfSharp.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PdfBatchEdit
{
    class BatchFile
    {
        private SourceFile source;
        private TargetFile target;

        public BatchFile() { }

        public static BatchFile FromPath(string path)
        {
            BatchFile file = new BatchFile();
            file.source = new SourceFile(path);
            return file;
        }

        public SourceFile Source
        {
            get { return source; }
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
            foreach (IPdfEffect effect in effects)
            {
                effect.ApplyEffect(document);
            }
            return document;
        }
    }
}
