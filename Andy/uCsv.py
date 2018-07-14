import os
import pandas as pd 
import numpy as np
from keras.preprocessing import text

class uCsv(object):
    """Utility functions for loading training dataset from a CSV file, and to extract
    the X variables and the ground truth y using column labels"""

    def fileExists(filename):
        os.path.exists(filename)

    def write(csvFile, npData, separator=","):
        pdData = pd.DataFrame(data=npData)
        pdData.to_csv(csvFile, sep=separator, index=False, header=False)#, index_label=False)

    def writePd(csvFile, pdData, separator=","):
        pdData.to_csv(csvFile, sep=separator, index=False, header=True)

    def openRead(csvfile, separator=","):
        data = pd.read_csv(csvfile, sep=separator)
        return data

    def getXFromColumns(train_ori, columnsToKeep):
        """To extract independant variables X
        Input types: pandas dataframe, and list of column labels to keep by removing other columns. Output type: 1 numpy array"""
        data = uCsv.keepColumns(train_ori, columnsToKeep)
        X = np.array(data.loc[:,:])
        return X

    def getXExceptColumns(train_ori, columnsToKeep):
        """To extract independant variables X
        Input types: pandas dataframe, and list of column labels to exclude. Output type: 1 numpy array"""
        data = uCsv.dropColumns(train_ori, columnsToKeep)
        X = np.array(data.loc[:,:])
        return X
    
    def getFlatYFromColumn(train_ori, columnsToKeep):
        """To extract dependant variable Y that contains the perfect prediction
        Input types: pandas dataframe, and list of column labels. Output type: 1 numpy array"""
        if len(columnsToKeep) <= 0:
            return None
        columnToKeep = columnsToKeep[0]
        data = uCsv.keepColumns(train_ori, columnsToKeep)
        y = np.ravel(data.loc[:,:])
        return y

    def dropColumns(train_ori, columnsToDrop):
        data = train_ori.drop(columnsToDrop, axis=1)#, "Fare"], axis=1)
        return data

    def keepColumns(train_ori, columnsToKeep):
        all_columns   = train_ori.columns.values.tolist()
        columnsToDrop = uCsv.removeFromListAElementsFoundinListB(all_columns, columnsToKeep)
        data = train_ori.drop(columnsToDrop, axis=1)#, "Fare"], axis=1)
        return data


    def removeFromListAElementsFoundinListB(A, B):
        """To ensure list A won't share any elements in common with list B.
        Input types: 2 lists. Output type: 1 list. This function has been optimized for speed."""
        # timed tests using:
        #a = range(1,500000)
        #b = range(1,100000)
        
        #1) comprehension method
        #comprehension   = [x for x in a if x not in b]    # 12.8 sec

        #2) filter_function method
        #filter_function = filter(lambda x: x not in b, a) # 12.6 sec

        #3) modification method
        copyA = A.copy()
        for x in B: # 0.27 sec
            try:
                copyA.remove(x)
            except ValueError:
                pass
        return copyA
    
    def listFilesWithExtensionsInDirectoryAndSubDirectories(exts, topDir):
        """ext: The extension to search for;
        topDir: The top directory"""
        listCsv = []
        for dirpath, dirnames, files in os.walk(topDir):
            for name in files:
                lowerName = name.lower()
                for ext in exts:
                    lowerExt = ext.lower()
                    if lowerName.endswith(lowerExt):
                        listCsv.append(os.path.join(dirpath, lowerName))
        return listCsv

    def listFilesWithExtensionInDirectoryAndSubDirectories(ext, topDir):
        """ext: The extension to search for;
        topDir: The top directory"""
        listCsv = []
        lowerExt = ext.lower()
        for dirpath, dirnames, files in os.walk(topDir):
            for name in files:
                lowerName = name.lower()
                if lowerName.endswith(lowerExt):
                    listCsv.append(os.path.join(dirpath, lowerName))
        return listCsv

    def listFilesWithExtButWithoutOtherExtsInDirectoryAndSubDirectories(extToInclude, extsToExclude, topDir):
        """extToInclude: The extension to search for, ex: file1.csv;
        extsToExclude: Similar extensions we don't want, ex: file1.predicted.csv;
        topDir: The top directory"""
        list1 = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories(extToInclude, topDir)
        list2 = uCsv.listFilesWithExtensionsInDirectoryAndSubDirectories(extsToExclude, topDir)
        list3 = uCsv.removeFromListAElementsFoundinListB(list1, list2)
        return list3

    def listFilesWithExtButWithoutAnotherExtInDirectoryAndSubDirectories(extToInclude, extToExclude, topDir):
        """extToInclude: The extension to search for, ex: file1.csv;
        extToExclude: A similar extension we don't want, ex: file1.predicted.csv;
        topDir: The top directory"""
        list1 = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories(extToInclude, topDir)
        list2 = uCsv.listFilesWithExtensionInDirectoryAndSubDirectories(extToExclude, topDir)
        list3 = uCsv.removeFromListAElementsFoundinListB(list1, list2)
        return list3
    
    def stackPandasRows(pdAccumulateArray, pdNewRows):
        """Generic function that can be called repeatedly to vertically stack more and more rows to a table.
        The pre-initialize the pdAccumulateArray to an empty array = []."""
        if pdAccumulateArray is None:
            pdAccumulateArray = []
        length = len(pdAccumulateArray)
        if length <= 0:
            pdAccumulateArray = pdNewRows
        else:
            pdAccumulateArray = pd.concat([pdAccumulateArray, pdNewRows], axis=0)
        return pdAccumulateArray

    def stackNumpyRows(npAccumulateArray, npNewRows):
        """Generic function that can be called repeatedly to vertically stack more and more rows to a table.
        The pre-initialize the npAccumulateArray to an empty array = []."""
        if npAccumulateArray is None:
            npAccumulateArray = []
        length = len(npAccumulateArray)
        if length <= 0:
            npAccumulateArray = npNewRows
        else:
            npAccumulateArray = np.concatenate((npAccumulateArray, npNewRows), axis=0)
        return npAccumulateArray

    def haveSameNbOfRows(pdArray1, pdArray2):
        """Check if 2 pandas dataframes have the same number of rows"""
        if pdArray1 is None or pdArray2 is None:
            return False
        return pdArray1.shape[0] == pdArray2.shape[0]

    def isValidArray(array):
        if array is None:
            return False
        elif len(array) <= 0:
            return False
        return True

