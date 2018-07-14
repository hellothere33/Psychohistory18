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
dirUncuratedCsvs = "Uncurated CSVs"
extCsv     = ".csv"
extMatched = ".matched"
extPredict = ".predicted.csv"
extUpload  = ".toUpload"
extToExclude = [extPredict, ".Matched.csv", ".ToUpload.csv", ".WrongPrediction.csv"]

#======================================================================================
# Load dataset
listUncurate = uCsv.listFilesWithExtButWithoutOtherExtsInDirectoryAndSubDirectories(extCsv, extToExclude, dirUncuratedCsvs) #relative paths. Ex: Uncurated CSVs\CMG Financial\locations.csv
listMatched  = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories(extMatched, dirUncuratedCsvs) #relative paths. Ex: Uncurated CSVs\CMG Financial\locations.matched
print("=> Found %i matched files" % len(listMatched))
print("=> Found %i uncurated files" % len(listUncurate))

if (not uCsv.isValidArray(listMatched)) or (not uCsv.isValidArray(listMatched)):
    print("No uncurated CSVs to predict and fill, exiting program.")
    exit()

#======================================================================================
# Load model to predict
model = None

print("Total files to predict: %s" % len(listUncurate))
nbFilesPredicted = 1
for uPath in listUncurate:
    print("   Predicting file # %s: %s" % (nbFilesPredicted, uPath))
    nbFilesPredicted += 1
    uBase = uStr.getFilePathWithoutExt(uPath)
    found = uStr.findMatchingFileNameAfterRemovingExt(uBase, listMatched, extMatched)
    if not found:
        continue # this uncurated csv doesn't have a corresponding matched file

    # Get both the uncurate and the matched CSVs
    csvUncurate = ImportData.readCurationCsvExportFile(uPath)
    csvMatched  = ImportData.readMatchedCsvFile(found)
    if not uCsv.haveSameNbOfRows(csvUncurate, csvMatched):
        continue

    # if predicted file already exists, then just load it; if not, calculate a new one
    listPredict  = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories(extPredict, dirUncuratedCsvs) #relative paths. Ex: Uncurated CSVs\CMG Financial\locations.predicted
    if uStr.findMatchingFileNameAfterRemovingExt(uBase, listPredict, extPredict):
        csvPredicted = ImportData.readCurationCsvExportFile(uBase+extPredict)
    else:
        X = uCsv.getXExceptColumns (csvMatched, ["verdict"])
        y = uCsv.getFlatYFromColumn(csvMatched, ["verdict"])
        if model is None:
            model = load_model("AiCurator Simple.model")
        yProb = model.predict(X, batch_size=128, verbose=2)
        yPdct = [float(np.round(x)) for x in yProb]
        #accuracy = np.mean(yPdct == y) # For debugging: verify the accuracy of the model against new manually curated CSVs
        #print("Prediction Accuracy: %.2f%%" % (accuracy*100))
        csvUncurate["Curation Status"] = ImportData.convertFloatToCurationStatus(yPdct)
        csvPredicted = csvUncurate
        uCsv.writePd(uBase+extPredict, csvUncurate)
        
    listPredict  = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories(extPredict, dirUncuratedCsvs) #relative paths. Ex: Uncurated CSVs\CMG Financial\locations.predicted
    listUpload   = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories(extUpload , dirUncuratedCsvs) #relative paths. Ex: Uncurated CSVs\CMG Financial\locations.toUpload
    foundCorrespondingPredictedFile = uStr.findMatchingFileNameAfterRemovingExt(uBase, listPredict, extPredict)
    foundCorrespondingToUploadFile  = uStr.findMatchingFileNameAfterRemovingExt(uBase, listUpload , extUpload )
    if foundCorrespondingPredictedFile and not foundCorrespondingToUploadFile:
        if not uCsv.isValidArray(csvPredicted):
            csvPredicted = ImportData.readCurationCsvExportFile(uBase+extPredict)
        # Export to a new .Upload csv file that's ready for upload to SWIQ, and contains 3 columns:
        # 'LOCATION_ID', 'UNIQUE_HASH', 'CURATION_STATUS' [verified or rejected]
        upload = pd.concat([csvPredicted["location_id"], csvPredicted["uniqueHash"], csvPredicted["Curation Status"]], axis=1)
        upload.rename(columns={'location_id': 'LOCATION_ID', 'uniqueHash': 'UNIQUE_HASH', 'Curation Status': 'CURATION_STATUS'}, inplace=True)
        uCsv.writePd(uBase+extUpload, upload)

print("Finished prediction of %s files" % len(listUncurate))
print("All done")

