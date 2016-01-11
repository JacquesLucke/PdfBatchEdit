import clr
clr.AddReference("PdfBatchEdit")
clr.AddReference("System")
import System
from PdfBatchEdit.Effects import TextEffect
from PdfBatchEdit import 

data = currentData

def GetArguments():

	return argsDict

def NewTextEffect():
    textEffect = TextEffect("")
    data.AddEffectToAllFiles(textEffect)
    return textEffect

def LoadFile(path):
    return data.AddFileWithAllEffects(path);
