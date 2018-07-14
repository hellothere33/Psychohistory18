import os
import re
from uDataGen import *
import numpy as np
from keras.preprocessing import text
from keras.preprocessing.text import Tokenizer

class uStr(object):
    """String manipulation, comparison, etc."""
    
    def compare2TextArrays(textArray1, textArray2, maxNbWords = 10):
        scoresArray = [] #np.zeros((textArray1.shape[0], maxNbWords), dtype=np.float)
        if not uStr.isValidWordArray(textArray1) or not uStr.isValidWordArray(textArray2):
            return scoresArray
        if len(textArray1) != len(textArray2):
            return scoresArray

        for i in range(len(textArray1)): # go through each row
            scores = uStr.compare2Texts(textArray1[i], textArray2[i], maxNbWords)
            if i is 0:
                scoresArray = scores
            else:
                scoresArray = np.vstack([scoresArray, scores])
        return scoresArray


    def compare2Texts(text1, text2, maxNbWords = 10):
        """Take 2 paragraphs of words, calculate for each word from paragraph1 the highest similarity score with words from paragraph2"""
        scores = np.zeros(maxNbWords, dtype=np.float)
        # validations
        if not uStr.isValidWordArray(text1) or not uStr.isValidWordArray(text2):
            return scores
        wds1 = uStr.text2Words(text1)
        wds2 = uStr.text2Words(text2)
        # if text1 is shorter than text2, switch to text2's words when text1's words run out
        length = min(maxNbWords, max(len(wds1), len(wds2)))
        for i in range(length): # go through each word
            if i < len(wds1):
                scores[i] = uStr.getHighestScore(wds1[i], wds2)
            else:
                scores[i] = uStr.getHighestScore(wds2[i], wds1)
        return scores

    def getHighestScore(word, wordArray, ignoreCaps = True):
        score = 0
        for w in wordArray:
            ratio = 0
            ratio = uStr.jaccardSimilarity(word, w)
            if score < ratio:
                score = ratio
        return score
        
    def text2Words(word, filters='!"#$%&()*+,-./:;<=>?@[\\]^_`{|}~\t\n', lower=False, split=" "):
        if not uStr.isValidWord(word):
            return []
        words = text.text_to_word_sequence(word, filters=filters, lower=lower, split=split)
        return words
    
    def word2HashInteger(word, max_nb_words, filters='!"#$%&()*+,-./:;<=>?@[\\]^_`{|}~\t\n', lower = False, split=" "):
        if not uStr.isValidWord(word):
            return []
        nb = text.hashing_trick(text=word,
                                n=max_nb_words,
                                hash_function='md5', # 'md5' is a stable hashing function consistent across different runs
                                filters=filters,
                                lower=lower,
                                split=split)
        return nb

    def text2HashIntegers(word, max_nb_words, filters='!"#$%&()*+,-./:;<=>?@[\\]^_`{|}~\t\n', lower = False, split=" "):
        if not uStr.isValidWord(word):
            return []
        nbs = text.hashing_trick(text=word,
                                 n=max_nb_words,
                                 hash_function='md5', # 'md5' is a stable hashing function consistent across different runs
                                 filters=filters,
                                 lower=lower,
                                 split=split)
        return nbs

    def textTokenizer(word, max_nb_words, filters='!"#$%&()*+,-./:;<=>?@[\\]^_`{|}~\t\n', lower=False, split=" "):
        """(not sure this really works, to be tested)"""
        nbs = text.Tokenizer(num_words=max_nb_words,
                                   filters=filters,
                                   lower=lower,
                                   split=split,
                                   char_level=False)
        return nbs

    def isValidWord(word):
        if (   word is None
            or not isinstance(word, str)
            or len(word) <= 0):
            return False
        return True
    
    def isValidWordArray(words):
        if (   words is None
            or isinstance(words, float) # sometimes NaN can show up in strings, because of an empty cell in Csv
            or isinstance(words, int)   # sometimes   0 can show up in strings, because of an empty cell in Csv
            or len(words) <= 0
            or len(words[0]) <= 0):
            return False
        return True

    def jaccardSimilarity(str1, str2):
        if not uStr.isValidWord(str1):
            return 0
        if not uStr.isValidWord(str2):
            return 0
        intersection = set(str1).intersection(set(str2))
        union = set(str1).union(set(str2))
        return len(intersection)/len(union)

    def replaceCaseInsensitive(text, old, new):
        str = re.sub("(?i)"+old,new, text)
        return str

    def findMatchingFileNameIgnoringExt(pathRef, listOfPathsToCompareAgainst):
        """Take the reference file's path, take a list of file paths, try to find of there's a file in the list of paths that matches the reference,
        but ignoring file extensions"""
        uBase = uStr.getFilePathWithoutExt(pathRef)
        foundPath = None
        for mPath in listOfPathsToCompareAgainst:
            mBase = uStr.getFilePathWithoutExt(mPath)
            if uBase == mBase:
                foundPath = mPath
                break
        return foundPath

    def findMatchingFileNameAfterRemovingExt(pathRef, listOfPathsToCompareAgainst, extToRemove):
        """Take the reference file's path, take a list of file paths, try to find of there's a file in the list of paths that matches the reference,
        but replacing file extensions with empty"""
        uBase = pathRef.lower()
        foundPath = None
        for mPath in listOfPathsToCompareAgainst:
            startWith = uStr.replaceCaseInsensitive(mPath.lower(), extToRemove, "")
            if startWith:
                if startWith == uBase:
                    foundPath = mPath
                    break
        return foundPath
    
    def findShortestMatchingFileNameStartingWith(refNameToStartWith, listOfPathsToCompareAgainst):
        """Take the reference file's path, take a list of file paths, try to find of there's a file in the list of paths that
        starts with the reference string"""
        uBase = refNameToStartWith#uStr.getFilePathWithoutExt(refNameToStartWith)
        minPathLength = 9999
        foundPath = None
        for mPath in listOfPathsToCompareAgainst:
            startWith = mPath.startswith(uBase)
            if startWith:
                if len(mPath) < minPathLength:
                    foundPath = mPath
                    minPathLength = len(mPath)
        return foundPath

    def findMatchingFileNameStartingWith(refNameToStartWith, listOfPathsToCompareAgainst):
        """Take the reference file's path, take a list of file paths, try to find of there's a file in the list of paths that
        starts with the reference string"""
        uBase = refNameToStartWith#uStr.getFilePathWithoutExt(refNameToStartWith)
        foundPath = None
        for mPath in listOfPathsToCompareAgainst:
            startWith = mPath.startswith(uBase)
            if startWith:
                foundPath = mPath
                break
        return foundPath

    def getFilePathWithoutExt(path):
        base = os.path.splitext(path)[0] # get entire path except the file's extension
        return base
    
    def getFileNameWithoutDirectories(path):
        base = os.path.basename(path) # get just filename without directories
        return base
