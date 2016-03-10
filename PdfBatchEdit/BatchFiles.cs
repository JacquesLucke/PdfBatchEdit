﻿using System.Collections.ObjectModel;
using System.IO;

namespace PdfBatchEdit
{
    public class BatchFiles : ObservableCollection<BatchFile>
    {
        public void Export(string targetDirectory)
        {
            foreach (BatchFile file in this)
            {
                string fileName = file.Source.NameWithExtension;
                if (file.UseCustomOutputName)
                    fileName = file.OutputName;

                string targetPath = Path.Combine(targetDirectory, fileName);
                file.Save(new GenericFile(targetPath));
            }
        }
    }
}
