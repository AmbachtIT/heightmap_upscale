import tensorflow as tf
import numpy as np
from tensorflow import keras
import os

TILE_SIZE_X = 64
TILE_SIZE_Y = 128

RESHAPED_X = TILE_SIZE_X * TILE_SIZE_X
RESHAPED_Y = TILE_SIZE_Y * TILE_SIZE_Y

DROPOUT = 0.3

DATA_PATH = "C:/Projects/heightmap_upscale/data/"
IMG_PATH = DATA_PATH + "8-bit/"
WEIGHTS_PATH = DATA_PATH + "weights.h5"

def createUpscaleModel():
    model = createModel_Upsample_SRCNN(TILE_SIZE_X, TILE_SIZE_Y, DROPOUT)
    if os.path.exists(WEIGHTS_PATH):
        print("Loading pre-trained weights...")
        model.load_weights(WEIGHTS_PATH)
    return model

# identity model, only containing a single activation layer. Used during refactoring
def createModel_Activation(TILESIZE_X, TILESIZE_Y, DROPOUT):
    RESHAPED_X = TILESIZE_X * TILESIZE_X
    model = tf.keras.models.Sequential()
    model.add(tf.keras.layers.InputLayer(input_shape=(RESHAPED_X, )))
    model.add(tf.keras.layers.Activation('relu'))
    model.compile(optimizer='Adam', loss='mse', metrics=['MeanSquaredError'])
    return model

# Use simple nearest neighbour upsampling. MSE:  0.00044283006
def createModel_Upsample_NearestNeighbour(TILESIZE_X, TILESIZE_Y, DROPOUT):
    RESHAPED_X = TILESIZE_X * TILESIZE_X
    model = tf.keras.models.Sequential()
    model.add(tf.keras.layers.InputLayer(input_shape=(RESHAPED_X, )))
    model.add(tf.keras.layers.Reshape( (TILESIZE_X, TILESIZE_X, 1) ))
    model.add(tf.keras.layers.UpSampling2D())
    model.add(tf.keras.layers.Flatten())
    model.compile(optimizer='Adam', loss='mse', metrics=['MeanSquaredError'])
    return model

    # Use simple bilinear upsampling. MSE:  0.00038636217
def createModel_Upsample_Bilinear(TILESIZE_X, TILESIZE_Y, DROPOUT):
    RESHAPED_X = TILESIZE_X * TILESIZE_X
    model = tf.keras.models.Sequential()
    model.add(tf.keras.layers.InputLayer(input_shape=(RESHAPED_X, )))
    model.add(tf.keras.layers.Reshape( (TILESIZE_X, TILESIZE_X, 1) ))
    model.add(tf.keras.layers.UpSampling2D(interpolation = 'bilinear'))
    model.add(tf.keras.layers.Flatten())
    model.compile(optimizer='Adam', loss='mse', metrics=['MeanSquaredError'])
    return model

# Use simple nearest neighbour upsampling plus a 2x2 convolutional layer. MSE:  0.0028712489
def createModel_Upsample_Conv2DTranspose(TILESIZE_X, TILESIZE_Y, DROPOUT):
    RESHAPED_X = TILESIZE_X * TILESIZE_X
    model = tf.keras.models.Sequential()
    model.add(tf.keras.layers.InputLayer(input_shape=(RESHAPED_X, )))
    model.add(tf.keras.layers.Reshape( (TILESIZE_X, TILESIZE_X, 1) ))
    model.add(tf.keras.layers.UpSampling2D())
    model.add(tf.keras.layers.Conv2DTranspose(1, (2, 2), padding='same'))
    model.add(tf.keras.layers.Flatten())
    model.compile(optimizer='Adam', loss='mse', metrics=['MeanSquaredError'])
    return model

# Use simple nearest neighbour upsampling, 9x9, 1x1 and 5x5 convolutional layers. MSE:  0.0028712489
# Described in https://towardsdatascience.com/an-evolution-in-single-image-super-resolution-using-deep-learning-66f0adfb2d6b
def createModel_Upsample_SRCNN(TILESIZE_X, TILESIZE_Y, DROPOUT):
    RESHAPED_X = TILESIZE_X * TILESIZE_X
    model = tf.keras.models.Sequential()
    model.add(tf.keras.layers.InputLayer(input_shape=(RESHAPED_X, )))
    model.add(tf.keras.layers.Reshape( (TILESIZE_X, TILESIZE_X, 1) ))
    model.add(tf.keras.layers.UpSampling2D(interpolation = 'bilinear'))
    model.add(tf.keras.layers.Conv2D(64, (9, 9), padding='same'))
    model.add(tf.keras.layers.Activation('relu'))
    model.add(tf.keras.layers.Conv2D(32, (1, 1), padding='same'))
    model.add(tf.keras.layers.Activation('relu'))
    model.add(tf.keras.layers.Conv2D(1, (5, 5), padding='same'))
    model.add(tf.keras.layers.Flatten())
    model.compile(optimizer='Adam', loss='mse', metrics=['MeanSquaredError'])
    return model