import os
os.environ["CUDA_VISIBLE_DEVICES"]="-1" # set to -1 to disable GPU
import numpy as np
import pandas as pd 
from uCsv import *
from uStr import *
from ImportData import *
from uAiModel import *
from sklearn.model_selection import train_test_split

from keras.preprocessing import text
from keras.layers import Dense, Activation
from keras.models import Sequential
from keras.callbacks import ReduceLROnPlateau, CSVLogger, TerminateOnNaN, ModelCheckpoint, EarlyStopping, TensorBoard

seed = 21
np.random.seed(seed) # fix random seed for reproducibility

#======================================================================================
# Load dataset
listMatched = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories('hypotheses.csv', 'NnInputs')
pdMatchedArray = []
for path in listMatched:
    csvData = ImportData.readMatchedCsvFile(path)
    pdMatchedArray = uCsv.stackPandasRows(pdMatchedArray, csvData)

y = uCsv.getFlatYFromColumn(pdMatchedArray, ["verdict"])
X = uCsv.getXExceptColumns (pdMatchedArray, ["verdict", "id"])
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.01, random_state=seed, shuffle=True, stratify=y)

#======================================================================================
# Build model(s) for a binary classification
#uAiModel.createArrayModels(X_train, X_test, y_train, y_test, batch_size = 32, epochs = 5) # using an array of models for comparison
uAiModel.createSimpleModel(X_train, X_test, y_train, y_test, batch_size = 2, epochs = 20) # using the best model from the array

print("All done")

