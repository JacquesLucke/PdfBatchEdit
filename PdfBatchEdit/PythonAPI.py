import clr
clr.AddReference("PdfBatchEdit")
from PdfBatchEdit.Effects import TextEffect

data = currentData

def newTextEffect():
    print "look"
    textEffect = TextEffect("")
    data.AddEffectToAllFiles(textEffect)
    return textEffect
