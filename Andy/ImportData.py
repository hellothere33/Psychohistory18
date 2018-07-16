#import types
import time
import numpy as np
import pandas as pd
import os.path
from uCsv import *
from uStr import *

class ImportData(object):
    """description of class"""
    def readMatchedCsvFile(csvfile, separator=","):
        """Returns a pandas dataframe of dimension nbRows X nbValueColumns """
        data = pd.read_csv(csvfile, sep=separator, dtype=np.float64)
        data[['ID_CPTE']] = data[['ID_CPTE']].astype(int)
        return data

    def readCurationCsvExportFile(csvfile, separator=","):
        """Returns a pandas dataframe of dimension nbRows X nbLabels """
        data = pd.read_csv(csvfile, sep=separator, dtype={'listing_id': str,
                                                          'location_id': str,
                                                          'uniqueHash': str,
                                                          'BranchId': str,
                                                          'Directory': str,
                                                          'Listing URL': str,
                                                          'Listing Name': str,
                                                          'Provided Name': str,
                                                          'Listing Name Match': None,#np.int32,
                                                          'Listing Civic': str,
                                                          'Provided Civic': str,
                                                          'Listing Civic Match': None,#np.int32,
                                                          'Listing Street Address': str,
                                                          'Provided Street Address': str,
                                                          'Listing Street Address Match': None,#np.int32,
                                                          'Listing Address': str,
                                                          'Location Address': str,
                                                          'Listing Address Match': None,#np.int32,
                                                          'Listing City': str,
                                                          'Provided City': str,
                                                          'Listing City Match': None,#np.int32,
                                                          'Listing Province/State': str,
                                                          'Provided Province/State': str,
                                                          'Listing Province/State Match': None,#np.int32,
                                                          'Listing Postal/Zip Code': str,
                                                          'Provided Postal/Zip Code': str,
                                                          'Listing Postal/Zip Code Match': None,#np.int32,
                                                          'Listing Country': str,
                                                          'Provided Country': str,
                                                          'Listing Country Match': None,#np.int32,
                                                          'Listing Phone': str,
                                                          'Provided Phone': str,
                                                          'Listing Phone Match': None,#np.int32,
                                                          'Listing Website': str,
                                                          'Provided Website': str,
                                                          'Listing Website Match': None,#np.int32,
                                                          'Average Match': None,#np.float64,
                                                          'Duplicate Status': str,
                                                          'Curation Status': str,
                                                         })
        #ImportData.curationDataCleanup(data)
        return data


    def curationDataCleanup(data):
        """Input type: pandas dataframe. Output type: pandas dataframe"""
        data.fillna(0, inplace=True) # NaN are replaced with this
        data['Curation Status'] = ImportData.convertCurationStatusToFloat(data)
        
    def calculateNewDataArraysOfAddressMatches(data):
        t1 = time.time()
        ArrayBizName    = ImportData.calculateAndSaveNewArray_OrLoadArrayFromExistingFile(data, "ArrayBizName.npy"   , "Listing Name", "Provided Name", maxNbWords = 8)
        t2 = time.time()
        ArrayBizCivic   = ImportData.calculateAndSaveNewArray_OrLoadArrayFromExistingFile(data, "ArrayBizCivic.npy"  , "Listing Civic", "Provided Civic", maxNbWords = 2)
        t3 = time.time()
        ArrayBizAddress = ImportData.calculateAndSaveNewArray_OrLoadArrayFromExistingFile(data, "ArrayBizAddress.npy", "Listing Address","Location Address", maxNbWords = 10)
        t4 = time.time()
        ArrayBizCity    = ImportData.calculateAndSaveNewArray_OrLoadArrayFromExistingFile(data, "ArrayBizCity.npy"   , "Listing City","Provided City", maxNbWords = 3)
        t5 = time.time()

        elaName = t2 - t1
        elaCivi = t3 - t2
        elaAddr = t4 - t3
        elaCity = t5 - t4

        #ArrayBizProvince= ImportData.convertToFloatArray(data['Listing Province/State Match'])
        #ArrayBizPostal  = ImportData.convertToFloatArray(data['Listing Postal/Zip Code Match'])
        
        print(ArrayBizName.shape)
        print(ArrayBizCivic.shape)
        print(ArrayBizAddress.shape)
        print(ArrayBizCity.shape)
        #print(ArrayBizProvince.shape)
        #print(ArrayBizPostal.shape)
        
        sumArrays = np.concatenate((ArrayBizName, ArrayBizCivic, ArrayBizAddress, ArrayBizCity), axis=1)#, ArrayBizProvince, ArrayBizPostal), axis=1)
        print(sumArrays.shape)
        return sumArrays

    def convertToFloatArray(columnData):
        length = int(len(columnData))
        arrayOut = np.zeros((length,1), dtype=np.float)
        for i in range(length):
            res = ImportData.clean_Numpy_Float(columnData[i])
            arrayOut[i] = res
        arrayOut = np.array(arrayOut)
        return arrayOut

    def calculateAndSaveNewArray_OrLoadArrayFromExistingFile(data, filename, columnName1, columnName2, maxNbWords = 10):
        if os.path.exists(filename):
            array = np.load(file=filename)
        else:
            array = uStr.compare2TextArrays(data[columnName1].values, data[columnName2].values, maxNbWords)
            np.save(filename, array)
        return array
        
    def assign_ListingName(data, label):
        vals = data.loc[:, label]
        for i in range(len(vals)):
            data.loc[i, label] = ImportData.clean_Text(vals[i])
            
    def clean_Text(val):
        """Make sure all values are float or integer"""
        if val is None or not isinstance(val, str):
            return ""
        words = uCsv.text2Words(val)
        return words

    def assign_Integer(data, label):
        vals = data.loc[:, label]
        for i in range(len(vals)):
            data.loc[i, label] = ImportData.clean_Numpy_Integer(vals[i])
            
    def assign_Float_Integer(data, label):
        vals = data.loc[:, label]
        for i in range(len(vals)):
            data.loc[i, label] = ImportData.clean_Numpy_Float_Integer(vals[i])

    def clean_Numpy_Float(val):
        """Make sure to convert any non-float to a default float value of 0.0"""
        if isinstance(val,int) or np.issubdtype(val, int):
            return float(val)
        if (not np.issubdtype(val, float)) and (not not np.issubdtype(val, float)):
            return float(0)
        return val

    def clean_Numpy_Integer(val):
        """Make sure to convert any non-interger to a default int value of 0"""
        if np.issubdtype(val, float):
            return int(val)
        if not np.issubdtype(val, int):
            return int(0)
        return val

    def convertCurationStatusToFloat(data):
        res = []
        array1 = data['Curation Status'].values
        for s in array1:
            if (not isinstance(s, str)) or (len(s) <= 0):
                res.append(0)
                continue
            s = s.lower()
            if s == 'verified':
                res.append(1)
            elif s == 'rejected':
                res.append(0)
        return res

    def convertFloatToCurationStatus(npVec):
        res = []
        array1 = npVec
        for v in array1:
            if isinstance(v, float) or isinstance(v, int):
                if int(v) == 1:
                    res.append("verified")
                    continue
                elif int(v) == 0:
                    res.append("rejected")
                    continue
            res.append("")
        return res

#listing_id, location_id, uniqueHash, BranchId, Directory, Listing URL,
#Listing Name, Provided Name, Listing Name Match,
#Listing Civic, Provided Civic, Listing Civic Match,
#Listing Street Address, Provided Street Address, Listing Street Address Match,
#Listing Address, Location Address, Listing Address Match,
#Listing City, Provided City, Listing City Match,
#Listing Province/State, Provided Province/State, Listing Province/State Match,
#Listing Postal/Zip Code, Provided Postal/Zip Code, Listing Postal/Zip Code Match,
#Listing Country, Provided Country, Listing Country Match,
#Listing Phone, Provided Phone, Listing Phone Match,
#Listing Website, Provided Website, Listing Website Match,
#Average Match, Duplicate Status, Curation Status