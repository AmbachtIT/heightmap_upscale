import tensorflow as tf
import numpy as np
import heightmap
import models
import util
from tensorflow import keras
import glob
import os
import png as png
import math

# Parameters
EPOCHS = 1
BATCH_SIZE = 128
VERBOSE = 1
VALIDATION_SPLIT = 0.2
TILESIZE = 32
IMG_PATH = "D:/Data/heightmap_upscale/4x/"
WEIGHTS_PATH = "weights4.h5"

model = models.createModel4(TILESIZE)
if(os.path.exists(WEIGHTS_PATH)):
    print("Weights found. Loading those.")
    model.load_weights(WEIGHTS_PATH)


X_train = util.readSamples(IMG_PATH + "train_x/")
Y_train = util.readSamples(IMG_PATH + "train_y/")
X_test = util.readSamples(IMG_PATH + "test_x/")
Y_test = util.readSamples(IMG_PATH + "test_y/")



while True:
    model.fit(X_train, Y_train, batch_size=BATCH_SIZE, epochs=EPOCHS, verbose=VERBOSE, validation_split=VALIDATION_SPLIT)
    model.save_weights(WEIGHTS_PATH)

    test_loss, test_acc = model.evaluate(X_test, Y_test)
    print('MSE: ', test_acc)

    ys = model.predict(X_test)
    ys += .5
    fileNames = []

    for file in sorted(glob.glob(IMG_PATH +"test_x/*.npy")):
        fileNames.append(os.path.split(file)[1])

    count = 0
    for y in ys:
        y = y.reshape(TILESIZE * 4, TILESIZE * 4)
        yt = Y_train[count].reshape(TILESIZE * 4, TILESIZE * 4)
        np.save(IMG_PATH + "predicted/{}".format(fileNames[count]), y)
        count = count + 1

    #    heightmap.arrayToPng(y, IMG_PATH + "tiles-predicted/{}".format(fileNames[count]))
    #    heightmap.arrayToPng(dif, IMG_PATH + "tiles-diff/{}".format(fileNames[count]))

    #    upscaled = None
    #    if count % 160 == 0:
    #        for i in range(1, 5):
                
    #            if i == 1:
    #                upscaled = heightmap.pngToArray(IMG_PATH + "upscaled-{}/{}".format(i, fileNames[count]))
    #                upscaled = upscaled.reshape( TILESIZE * 2, TILESIZE * 2 )
    #            else:
    #                upscaled = upscale(model, upscaled)
    #                array = upscaled.flatten()
    #                heightmap.arrayToPng(array, IMG_PATH + "upscaled-{}/{}".format(i, fileNames[count]))

    #    count = count + 1


    





# count = 0
# for x in X_train:
#     y = model.predict(x)

#     arrayToPng(y, "C:/Projects/heightmap_upscale/data/8-bit/tiles-predicted/{}.png".format(count))



