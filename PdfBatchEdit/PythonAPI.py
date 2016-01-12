import clr
clr.AddReference("System")
clr.AddReference("PdfBatchEdit")

import System
import PdfBatchEdit
from PdfBatchEdit.Effects import TextEffect

data = currentData

def GetArguments():
	return PdfBatchEdit.Utils.GetArgumentsDictionary()

def NewTextEffect():
    textEffect = TextEffect("")
    data.AddEffectToAllFiles(textEffect)
    return textEffect

def LoadFile(path):
    return data.AddFileWithAllEffects(path);
