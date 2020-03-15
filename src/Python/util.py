import tensorflow as tf
import numpy as np
from tensorflow import keras
import glob
import os

TILE_SIZE_INPUT = 64
RESHAPED_INPUT = TILE_SIZE_INPUT * TILE_SIZE_INPUT

def readSamples(path):
    list = []
    files = glob.glob(path + "*.npy")
    for path in files:
        arr = np.load(path)
        arr -= .5
        list.append(arr.flatten())
    return np.stack(list)

    


    