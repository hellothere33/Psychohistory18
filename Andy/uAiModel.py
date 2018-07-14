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

from keras.preprocessing import text
from keras.layers import Dense, Activation
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

    def createSimpleModel(X_train, X_test, y_train, y_test, batch_size = 128, epochs = 20):
        Xcols = X_train.shape[1] # number of columns, can vary depending on the how the data was constructed and embedded
        #model = Sequential([Dense( 8, input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')])
        print("------------------------------------------------------------------")
        print("Training with Neural Network model: D48 D24 D16 D8 D1")
        model = Sequential([Dense(48  , input_shape=(Xcols,)), Activation('relu'), Dense(24, input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')])
        model.compile(loss='binary_crossentropy', optimizer='adam', metrics=['accuracy'])
        model.fit(X_train, y_train, validation_data=(X_test, y_test), epochs=epochs, batch_size=batch_size, verbose=1)
        model.save("AiCurator Simple.model")

    def createArrayModels(X_train, X_test, y_train, y_test, batch_size = 128, epochs = 20):
        Xcols = X_train.shape[1] # number of columns, can vary depending on the how the data was constructed and embedded
        models = []
        models.append(("D20   D20  D8   D1" , [Dense(20  , input_shape=(Xcols,)), Activation('relu'), Dense(20 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D24   D20  D8   D1" , [Dense(24  , input_shape=(Xcols,)), Activation('relu'), Dense(20 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D32   D20  D8   D1" , [Dense(32  , input_shape=(Xcols,)), Activation('relu'), Dense(20 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D40   D16  D8   D1" , [Dense(40  , input_shape=(Xcols,)), Activation('relu'), Dense(16 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D40   D24  D8   D1" , [Dense(40  , input_shape=(Xcols,)), Activation('relu'), Dense(24 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D40   D32  D8   D1" , [Dense(40  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D48   D16  D8   D1" , [Dense(48  , input_shape=(Xcols,)), Activation('relu'), Dense(16 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D48   D24  D8   D1" , [Dense(48  , input_shape=(Xcols,)), Activation('relu'), Dense(24 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D48   D16  D16  D1" , [Dense(48  , input_shape=(Xcols,)), Activation('relu'), Dense(16 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D24  D8   D1" , [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(24 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D32  D8   D1" , [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D32  D16  D1" , [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D32  D16  D1" , [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D96   D24  D8   D1" , [Dense(96  , input_shape=(Xcols,)), Activation('relu'), Dense(24 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D96   D32  D8   D1" , [Dense(96  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D96   D32  D16  D1" , [Dense(96  , input_shape=(Xcols,)), Activation('relu'), Dense(32 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D96   D48  D16  D1" , [Dense(96  , input_shape=(Xcols,)), Activation('relu'), Dense(48 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D48  D8   D1" , [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(48 ,input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D48  D16  D1" , [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(48 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D128  D64  D16  D1" , [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(64 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D96  D32  D1" , [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256  D128 D32  D1" , [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256  D128 D64  D1" , [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512  D128 D32  D1" , [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512  D256 D64  D1" , [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D500 D128 D32 D1" , [Dense(500 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
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
        models.append(("D20   D20  D20  D4  D1", [Dense(20  , input_shape=(Xcols,)), Activation('relu'), Dense(20, input_shape=(Xcols,)), Activation('relu'), Dense(20 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D32   D20  D16  D8  D1", [Dense(32  , input_shape=(Xcols,)), Activation('relu'), Dense(20, input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D40   D20  D16  D8  D1", [Dense(40  , input_shape=(Xcols,)), Activation('relu'), Dense(20, input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D48   D24  D16  D8  D1", [Dense(48  , input_shape=(Xcols,)), Activation('relu'), Dense(24, input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D64   D32  D16  D8  D1", [Dense(64  , input_shape=(Xcols,)), Activation('relu'), Dense(32, input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D128  D64  D16  D4  D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(64 ,input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D128  D96  D32  D8  D1", [Dense(128 , input_shape=(Xcols,)), Activation('relu'), Dense(96 ,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256  D128 D32  D4  D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D256  D128 D64  D8  D1", [Dense(256 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        models.append(("D512  D128 D32  D4  D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D512  D256 D64  D8  D1", [Dense(512 , input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768  D128 D32  D4  D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(128,input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(4  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D768  D256 D64  D8  D1", [Dense(768 , input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(8  , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D1024 D256 D64  D16 D1", [Dense(1024, input_shape=(Xcols,)), Activation('relu'), Dense(256,input_shape=(Xcols,)), Activation('relu'), Dense(64 , input_shape=(Xcols,)), Activation('relu'), Dense(16 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))
        # models.append(("D1024 D512 D128 D32 D1", [Dense(1024, input_shape=(Xcols,)), Activation('relu'), Dense(512,input_shape=(Xcols,)), Activation('relu'), Dense(128, input_shape=(Xcols,)), Activation('relu'), Dense(32 , input_shape=(Xcols,)), Activation('relu'), Dense(1), Activation('sigmoid')]))

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