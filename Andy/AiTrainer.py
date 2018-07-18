import os
os.environ["CUDA_VISIBLE_DEVICES"]="-1" # set to -1 to disable GPU
import numpy as np
import pandas as pd 
from uCsv import *
from uStr import *
from ImportData import *
from uAiModel import *
from sklearn.model_selection import train_test_split
from sklearn.metrics import roc_auc_score

from keras.preprocessing import text
from keras.layers import Dense, Activation
from keras.models import Sequential, load_model
from keras.callbacks import ReduceLROnPlateau, CSVLogger, TerminateOnNaN, ModelCheckpoint, EarlyStopping, TensorBoard

seed = 21
np.random.seed(seed) # fix random seed for reproducibility
columnDefaultName = "Default"
columnID_CPTEName = "ID_CPTE"

#======================================================================================
# Load dataset
listMatched = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories('hypotheses_train - Andy.csv', 'NnInputs')
pdMatchedArray = []
for path in listMatched:
    csvData = ImportData.readMatchedCsvFile(path)
    pdMatchedArray = uCsv.stackPandasRows(pdMatchedArray, csvData)

y = uCsv.getFlatYFromColumn(pdMatchedArray, [columnDefaultName])
X = uCsv.getXExceptColumns (pdMatchedArray, [columnDefaultName, columnID_CPTEName])
X_train, X_test, y_train, y_test = train_test_split(X, y, test_size=0.30, random_state=seed, shuffle=True, stratify=y)

#======================================================================================
# Build model(s) for a binary classification
#uAiModel.createArrayModels(X_train, X_test, y_train, y_test, batch_size = 64, epochs = 50) # using an array of models for comparison
uAiModel.createSimpleModel(X_train, X_test, y_train, y_test, batch_size = 32, epochs = 30, auc_at_epoch_nb = 1) # using the best model from the array

#======================================================================================
# Calculate the performance score (Area Under Curve of ROC) of the model
#model = load_model("AiPredictor Simple.model")
#PredictAndCalculateAUCScore(X_train, y_train)
#PredictAndCalculateAUCScore(X_test , y_test )
#print("All done")


def PredictAndCalculateAUCScore(X, y):
    yProb = model.predict(X, batch_size=128, verbose=2)
    yPdct = [float(np.round(x)) for x in yProb] # round float to either 0.0 or 1.0, it's as if threshold == 0.5
    auc_Probability = roc_auc_score(y, yProb, average="macro", sample_weight=None)
    auc_Prediction  = roc_auc_score(y, yPdct, average="macro", sample_weight=None)
    print("auc_Probability (0.0 ~ 1.0) = %s" % auc_Probability)
    print("auc_Prediction  (0 or 1) = %s" % auc_Prediction)