import os
os.environ["CUDA_VISIBLE_DEVICES"]="-1" # set to -1 to disable GPU
import numpy as np
import pandas as pd 
from uCsv import *
from uStr import *
from ImportData import *
from uAiModel import *

from keras.preprocessing import text
from keras.layers import Dense, Activation
from keras.models import Sequential, load_model
from keras.callbacks import ReduceLROnPlateau, CSVLogger, TerminateOnNaN, ModelCheckpoint, EarlyStopping, TensorBoard

seed = 2
np.random.seed(seed) # fix random seed for reproducibility
columnDefaultName = "Default"
columnID_CPTEName = "ID_CPTE"

#======================================================================================
# Load dataset
path    = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories("hypotheses_test - Andy.csv", "NnInputs")[0]
if not path:
    exit()
# Get both the uncurate and the matched CSVs
csvMatched  = ImportData.readMatchedCsvFile(path)

#======================================================================================
# Load model to predict
model = None
X = uCsv.getXExceptColumns(csvMatched, [columnDefaultName, columnID_CPTEName])
model = load_model("AiPredictor Simple.model")
if model is None:
    exit()

yProb = model.predict(X, batch_size=128, verbose=2)
yPdct = [float(np.round(x)) for x in yProb] # round float to either 0.0 or 1.0, it's as if threshold == 0.5
csvMatched[columnDefaultName] = yProb
solution = uCsv.keepColumns(csvMatched, [columnID_CPTEName, columnDefaultName])
#uCsv.writePd(path, csvMatched)
uCsv.writePd('NnInputs\\submission - andy.csv', solution)
        

print("All done")

