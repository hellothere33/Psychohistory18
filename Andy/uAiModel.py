import os
import numpy as np
from numpy import ndarray
import pandas as pd 
from uCsv import *
from uStr import *
from uDataGen import *
from uAiModel import *
from uHistory import *
from ImportData import *
from sklearn.model_selection import train_test_split
from sklearn.metrics import roc_auc_score

from keras.preprocessing import text
from keras.layers import Dense, Activation, Dropout, MaxPooling1D, MaxPooling2D, LSTM, Conv1D, MaxPool1D, Reshape
from keras.models import Sequential
from keras.callbacks import ReduceLROnPlateau, CSVLogger, TerminateOnNaN, ModelCheckpoint, EarlyStopping, TensorBoard

class uAiModel(object):
    """description of class"""

    def setThingsUp():
        # Prepare directories to contain training data
        uAiModel.createDirIfNotExist("./models")

    def createDirIfNotExist(directory):
        if not os.path.exists(directory):
            os.makedirs(directory)

    def createSimpleModel(X_train, X_test, y_train, y_test, batch_size = 128, epochs = 20, auc_at_epoch_nb = None):
        Xcols = X_train.shape[1] # number of columns, can vary depending on the how the data was constructed and embedded
        print("------------------------------------------------------------------")
        print("Training with Neural Network model: D2048 Drop50 D512 Drop50 D128 Drop50 D64 Drop50 D4 D1")
        model = Sequential([Dense(2048 , input_shape=(Xcols,)), Activation('relu'),
							Dropout(rate=0.50),
							Dense(512 , input_shape=(Xcols,)), Activation('relu'), 
							Dropout(rate=0.50),
							Dense(128,input_shape=(Xcols,)), Activation('relu'),
							Dense(128,input_shape=(Xcols,)), Activation('relu'),
							Dense(128,input_shape=(Xcols,)), Activation('relu'),
							Dropout(rate=0.50),
							Dense(64 , input_shape=(Xcols,)), Activation('relu'),
							Dropout(rate=0.50),
							Dense(4  , input_shape=(Xcols,)), Activation('relu'),
							Dense(1), Activation('sigmoid')])
       # model = Sequential([Conv1D (kernel_size = (200), filters = 1, input_shape=(Xcols,)), Activation('relu'),
       #                     MaxPooling1D(pool_size = (20), strides=(10)),
       #                     Dense(512,input_shape=(Xcols,)), Activation('relu'),
       #                     Dense(512,input_shape=(Xcols,)), Activation('relu'),
							#Dense(1), Activation('sigmoid')])
        model.compile(loss='binary_crossentropy', optimizer='adam', metrics=['accuracy'])
        if not auc_at_epoch_nb or auc_at_epoch_nb is 0:
            model.fit(X_train, y_train, validation_data=(X_test, y_test), epochs=epochs, batch_size=batch_size, verbose=1)
        else:
            for i in range(0, epochs, auc_at_epoch_nb):
                model.fit(X_train, y_train, validation_data=(X_test, y_test), epochs=auc_at_epoch_nb, batch_size=batch_size, verbose=1)
                uAiModel.PredictAndCalculateAUCScore(model, X_test, y_test)
        model.save("AiPredictor Simple.model")


    def PredictAndCalculateAUCScore(compiled_model, X, y):
        yProb = compiled_model.predict(X, batch_size=128, verbose=2)
        yPdct = [float(np.round(x)) for x in yProb] # round float to either 0.0 or 1.0, it's as if threshold == 0.5
        auc_Probability = roc_auc_score(y, yProb, average="macro", sample_weight=None)
        auc_Prediction  = roc_auc_score(y, yPdct, average="macro", sample_weight=None)
        print("auc_Probability (0.0 ~ 1.0) = %s" % auc_Probability)
        print("auc_Prediction  (0 or 1) = %s" % auc_Prediction)

    def createArrayModels(X_train, X_test, y_train, y_test, batch_size = 128, epochs = 20):
        Xcols = X_train.shape[1] # number of columns, can vary depending on the how the data was constructed and embedded
        models = []
        # models.append(("D20   D20  D8   D1" , [Dense(20  , input_shape=(Xcols,)), Activation('relu'), Dense(20 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D24   D20  D8   D1" , [Dense(24  , input_shape=(Xcols,)), Activation('relu'), Dense(20 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D32   D20  D8   D1" , [Dense(32  , input_shape=(Xcols,)), Activation('relu'), Dense(20 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D40   D16  D8   D1" , [Dense(40  , input_shape=(Xcols,)), Activation('relu'), Dense(16 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D40   D24  D8   D1" , [Dense(40  , input_shape=(Xcols,)), Activation('relu'), Dense(24 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D40   D32  D8   D1" , [Dense(40  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D48   D16  D8   D1" , [Dense(48  , input_shape=(Xcols,)), Activation('relu'), Dense(16 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D48   D24  D8   D1" , [Dense(48  , input_shape=(Xcols,)), Activation('relu'), Dense(24 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D48   D16  D16  D1" , [Dense(48  , input_shape=(Xcols,)), Activation('relu'), Dense(16 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D24  D8   D1" , [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(24 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D32  D8   D1" , [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D32  D16  D1" , [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D32  D16  D1" , [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D96   D24  D8   D1" , [Dense(96  , input_shape=(Xcols,)), Activation('relu'), Dense(24 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D96   D32  D8   D1" , [Dense(96  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D96   D32  D16  D1" , [Dense(96  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D96   D48  D16  D1" , [Dense(96  , input_shape=(Xcols,)), Activation('relu'), Dense(48 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D48  D8   D1" , [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(48 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D48  D16  D1" , [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(48 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D64  D16  D1" , [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(64 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D96  D32  D1" , [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256  D128 D32  D1" , [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256  D128 D64  D1" , [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512  D128 D32  D1" , [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512  D256 D64  D1" , [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D500 D128 D32 D1" , [Dense(500 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768  D64  D16  D1" , [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(64 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768  D128 D32  D1" , [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768  D256 D64  D1" , [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D1024 D128 D32  D1" , [Dense(1024, input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D1024 D256 D64  D1" , [Dense(1024, input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D1024 D512 D128 D1" , [Dense(1024, input_shape=(Xcols,)), Activation('relu'), Dense(512,input_shape=(Xcols,)), Activation('relu'), Dense(128, input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D2048 D128 D32  D1" , [Dense(2048, input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D2048 D256 D64  D1" , [Dense(2048, input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D2048 D512 D128 D1" , [Dense(2048, input_shape=(Xcols,)), Activation('relu'), Dense(512,input_shape=(Xcols,)), Activation('relu'), Dense(128, input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        # models.append(("D16   D20  D20  D4  D1", [Dense(20  , input_shape=(Xcols,)), Activation('relu'), Dense(20, input_shape=(Xcols,)), Activation('relu'), Dense(20 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D20   D20  D20  D4  D1", [Dense(20  , input_shape=(Xcols,)), Activation('relu'), Dense(20, input_shape=(Xcols,)), Activation('relu'), Dense(20 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D32   D20  D16  D8  D1", [Dense(32  , input_shape=(Xcols,)), Activation('relu'), Dense(20, input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D40   D20  D16  D8  D1", [Dense(40  , input_shape=(Xcols,)), Activation('relu'), Dense(20, input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D48   D24  D16  D8  D1", [Dense(48  , input_shape=(Xcols,)), Activation('relu'), Dense(24, input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D32  D16  D8  D1", [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(32, input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D64  D16  D4  D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(64 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D96  D32  D8  D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256  D128 D32  D4  D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256  D128 D64  D8  D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512  D128 D32  D4  D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512  D256 D64  D8  D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768  D128 D32  D4  D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768  D256 D64  D8  D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D1024 D256 D64  D16 D1", [Dense(1024, input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D1024 D512 D128 D32 D1", [Dense(1024, input_shape=(Xcols,)), Activation('relu'), Dense(512,input_shape=(Xcols,)), Activation('relu'), Dense(128, input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        
        # models.append(("D128 Drop10 D96  D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 Drop10 D128 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 Drop10 D128 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 Drop10 D128 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        # models.append(("D128 Drop20 D96  D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 Drop20 D128 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 Drop20 D128 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 Drop20 D128 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        # models.append(("D128 Drop30 D96  D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 Drop30 D128 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 Drop30 D128 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 Drop30 D128 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        # models.append(("D128 D96  Drop10 D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 D128 Drop10 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 D128 Drop10 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 D128 Drop10 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        # models.append(("D128 D96  Drop20 D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 D128 Drop20 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 D128 Drop20 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 D128 Drop20 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        # models.append(("D128 D96  Drop30 D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 D128 Drop30 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 D128 Drop30 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 D128 Drop30 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
                                                                         
        # models.append(("D128 D96  D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 D128 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 D128 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 D128 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        # models.append(("D128 Drop10 D96  D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 Drop10 D128 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 Drop10 D128 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 Drop10 D128 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
                                                                                                      
        # models.append(("D128 Drop20 D96  D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 Drop20 D128 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 Drop20 D128 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 Drop20 D128 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
                                                                                                      
        # models.append(("D128 Drop30 D96  D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 Drop30 D128 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 Drop30 D128 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 Drop30 D128 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        # models.append(("D128 D96  Drop10 D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 D128 Drop10 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 D128 Drop10 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 D128 Drop10 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
                                                                                                                                                                   
        # models.append(("D128 D96  Drop20 D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 D128 Drop20 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 D128 Drop20 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 D128 Drop20 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
                                                                                                                                                                   
        # models.append(("D128 D96  Drop30 D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 D128 Drop30 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 D128 Drop30 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 D128 Drop30 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        # models.append(("D128 Drop10 D96  Drop10 D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 Drop10 D128 Drop10 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 Drop10 D128 Drop10 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 Drop10 D128 Drop10 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(24 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.10), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
                                                                                                                                                                          
        # models.append(("D128 Drop20 D96  Drop20 D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 Drop20 D128 Drop20 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 Drop20 D128 Drop20 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 Drop20 D128 Drop20 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.20), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
                                                                                                                                                                          
        # models.append(("D128 Drop30 D96  Drop30 D32 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256 Drop30 D128 Drop30 D32 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 Drop30 D128 Drop30 D32 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768 Drop30 D128 Drop30 D32 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        models.append(("D128 Drop30 D64 Drop30 D96  Drop30 D32 Drop30 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D256 Drop30 D64 Drop30 D128 Drop30 D32 Drop30 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D512 Drop30 D64 Drop30 D128 Drop30 D32 Drop30 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D768 Drop30 D64 Drop30 D128 Drop30 D32 Drop30 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        models.append(("D128 Drop40 D64 Drop40 D96  Drop40 D32 Drop40 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D256 Drop40 D64 Drop40 D128 Drop40 D32 Drop40 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D512 Drop40 D64 Drop40 D128 Drop40 D32 Drop40 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D768 Drop40 D64 Drop40 D128 Drop40 D32 Drop40 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        models.append(("D128 Drop50 D64 Drop50 D96  Drop50 D32 Drop50 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D256 Drop50 D64 Drop50 D128 Drop50 D32 Drop50 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D512 Drop50 D64 Drop50 D128 Drop50 D32 Drop50 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D768 Drop50 D64 Drop50 D128 Drop50 D32 Drop50 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.50), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

        models.append(("D128 Drop60 D64 Drop60 D96  Drop60 D32 Drop60 D8 D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D256 Drop60 D64 Drop60 D128 Drop60 D32 Drop60 D4 D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D512 Drop60 D64 Drop60 D128 Drop60 D32 Drop60 D4 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D768 Drop60 D64 Drop60 D128 Drop60 D32 Drop60 D4 D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.60), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))


        # models.append(("D512 MaxP2-1 D128 MaxP2-1 D32 D4 Drop30 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), MaxPooling1D(pool_size=2, strides=1), Dense(128,input_shape=(Xcols,)), Activation('relu'), MaxPooling1D(pool_size=2, strides=1), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.30), Dense(4, input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512 MaxP2-2 D128 MaxP2-2 D32 D4 Drop40 D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), MaxPooling1D(pool_size=2, strides=2), Dense(128,input_shape=(Xcols,)), Activation('relu'), MaxPooling1D(pool_size=2, strides=2), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dropout(rate=0.40), Dense(4, input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))		

        uAiModel.setThingsUp()
        nbModels = len(models)
        histories  = []
        csvData = addHeaderToArray("ModelName", addHeaderToArray("Metric", np.arange(epochs)))

        for i in range(nbModels):
            tup = models[i]
            name  = tup[0]
            print("------------------------------------------------------------------")
            print("Training with Neural Network model: %s" % name)
            model = Sequential(tup[1])
            model.compile(loss='binary_crossentropy', optimizer='adam', metrics=['accuracy'])

            # Call backs
            reduce_lr = ReduceLROnPlateau(monitor='val_loss', factor=0.2, patience=3, min_lr=0.001)
            modelCheckpoint = ModelCheckpoint(filepath='./models/%s.epoch{epoch:02d}.acc{val_acc:.4f}.model.hdf5' % name, monitor='val_acc',
                                              verbose=0, save_best_only=True, save_weights_only=False, mode='auto', period=1)
            tensorBoard = TensorBoard(log_dir='./logs', histogram_freq=0, batch_size=32, write_graph=True, write_grads=False,
                                      write_images=False, embeddings_freq=0, embeddings_layer_names=None, embeddings_metadata=None)
            #terminateOnNaN = TerminateOnNaN()
            #earlyStopping = EarlyStopping(monitor='val_loss', min_delta=0, patience=0, verbose=0, mode='auto')
            #csv_logger = CSVLogger('%s - training log.csv' % name)

            # Run fitting
            history = model.fit(X_train, y_train, validation_data=(X_test, y_test), epochs=epochs, batch_size=batch_size, verbose=1,
                                callbacks=[reduce_lr, modelCheckpoint, tensorBoard])#, csv_logger, terminateOnNaN, earlyStopping])#

            # Record historical metrics
            #histories.append((name, history))
            a=addHeaderToArray(name, addHeaderToArray("train acc" , history.history['acc'])     )
            b=addHeaderToArray(name, addHeaderToArray("train loss", history.history['loss'])    )
            c=addHeaderToArray(name, addHeaderToArray("test acc"  , history.history['val_acc']) )
            d=addHeaderToArray(name, addHeaderToArray("test loss" , history.history['val_loss']))
            csvData = np.vstack([csvData, a, b, c, d]) #same as: csvData = np.stack((csvData, a, b, c, d), axis=0)
            #uHistory.viewHistoryAcc(history)
            #uHistory.viewHistoryLoss(history)
        
        #histories = np.array(histories)
        uCsv.write("AccLoss - all models.csv", csvData)
        np.save("AccLoss - all models.npy", csvData) 

def addHeaderToArray(header, vals):
    arr = [header]
    for v in vals:
        arr.append(v)
    return arr