import clr
clr.AddReference("PdfBatchEdit")
from PdfBatchEdit.Effects import TextEffect

data = currentData

def NewTextEffect():
    textEffect = TextEffect("")
    data.AddEffectToAllFiles(textEffect)
    return textEffect

def LoadFile(path):
    return data.AddFileWithAllEffects(path);
