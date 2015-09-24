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
            GenericFile file = new GenericFile(getNewPreviewPath());
            file.EnsureDirectory();
            PdfDocument document = ApplyEffects(effects);
            document.Save(file.Path);
            return file;
        }

        private string getNewPreviewPath()
        {
            string chars = "abcdefghijklmnopqrstuvwxyz";
            int length = 10;
            string name = "";
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                name += chars[random.Next(chars.Length)];
            }
            return AppDomain.CurrentDomain.BaseDirectory + "previews\\" + name + ".pdf";
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
