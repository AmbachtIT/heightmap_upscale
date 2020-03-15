import tensorflow as tf
import numpy as np
from tensorflow import keras

# Use simple nearest neighbour upsampling, 9x9, 1x1 and 5x5 convolutional layers. MSE:  0.0028712489
# Described in https://towardsdatascience.com/an-evolution-in-single-image-super-resolution-using-deep-learning-66f0adfb2d6b
# Article: https://arxiv.org/pdf/1501.00092.pdf
def createModel4(TILESIZE_INPUT):
    model = tf.keras.models.Sequential()
    model.add(tf.keras.layers.InputLayer(input_shape=(TILESIZE_INPUT * TILESIZE_INPUT, )))
    model.add(tf.keras.layers.Reshape( (TILESIZE_INPUT, TILESIZE_INPUT, 1) ))
    model.add(tf.keras.layers.UpSampling2D(interpolation = 'bilinear'))
    model.add(tf.keras.layers.UpSampling2D(interpolation = 'bilinear'))
    model.add(tf.keras.layers.Conv2D(64, (9, 9), padding='same'))
    model.add(tf.keras.layers.Activation('relu'))
    model.add(tf.keras.layers.Conv2D(32, (1, 1), padding='same'))
    model.add(tf.keras.layers.Activation('relu'))
    model.add(tf.keras.layers.Conv2D(1, (5, 5), padding='same'))
    model.add(tf.keras.layers.Flatten())
    model.compile(optimizer='Adam', loss='mse', metrics=['MeanSquaredError'])
    return model


    # Use simple nearest neighbour upsampling, 9x9, 1x1 and 5x5 convolutional layers. MSE:  0.0028712489
# Described in https://towardsdatascience.com/an-evolution-in-single-image-super-resolution-using-deep-learning-66f0adfb2d6b
# Article: https://arxiv.org/pdf/1501.00092.pdf
def createModel2(TILESIZE_INPUT):
    model = tf.keras.models.Sequential()
    model.add(tf.keras.layers.InputLayer(input_shape=(TILESIZE_INPUT * TILESIZE_INPUT, )))
    model.add(tf.keras.layers.Reshape( (TILESIZE_INPUT, TILESIZE_INPUT, 1) ))
    model.add(tf.keras.layers.UpSampling2D(interpolation = 'bilinear'))
    model.add(tf.keras.layers.Conv2D(64, (9, 9), padding='same'))
    model.add(tf.keras.layers.Dropout(DROPOUT))
    model.add(tf.keras.layers.Activation('relu'))
    model.add(tf.keras.layers.Conv2D(32, (1, 1), padding='same'))
    model.add(tf.keras.layers.Dropout(DROPOUT))
    model.add(tf.keras.layers.Activation('relu'))
    model.add(tf.keras.layers.Conv2D(1, (5, 5), padding='same'))
    model.add(tf.keras.layers.Dropout(DROPOUT))
    model.add(tf.keras.layers.Flatten())
    model.compile(optimizer='Adam', loss='mse', metrics=['MeanSquaredError'])
    return model