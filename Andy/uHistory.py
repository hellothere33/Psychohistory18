from keras.models import Sequential
from keras.layers import Dense
import matplotlib.pyplot as plt
import numpy as np


class uHistory(object):
    """Visualize Keras model training histories"""

    def viewHistory(history):
        """List all data in history"""
        print(history.history.keys())
        
    def viewHistoryAcc(history):
        """Summarize history for accuracy"""
        plt.plot(history.history['acc'])
        plt.plot(history.history['val_acc'])
        plt.title('model accuracy')
        plt.ylabel('accuracy')
        plt.xlabel('epoch')
        plt.legend(['train', 'test'], loc='upper left')
        plt.show()
        
    def viewHistoryLoss(history):
        """Summarize history for loss"""
        plt.plot(history.history['loss'])
        plt.plot(history.history['val_loss'])
        plt.title('model loss')
        plt.ylabel('loss')
        plt.xlabel('epoch')
        plt.legend(['train', 'test'], loc='upper left')
        plt.show()


