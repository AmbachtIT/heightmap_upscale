import tensorflow as tf
import numpy as np
import heightmap
import models
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
DROPOUT = 0.3

DATA_PATH = "C:/Projects/heightmap_upscale/data/"
IMG_PATH = DATA_PATH + "8-bit/"
WEIGHTS_PATH = DATA_PATH + "weights.h5"

TILESIZE = 64

X_train = heightmap.loadHeightmaps(IMG_PATH + "tiles-training-x/")
Y_train = heightmap.loadHeightmaps(IMG_PATH + "tiles-training-y/")
X_test = heightmap.loadHeightmaps(IMG_PATH + "tiles-test-x/")
Y_test = heightmap.loadHeightmaps(IMG_PATH + "tiles-test-y/")

model = models.createUpscaleModel()

# copy an image to another image
def blit(source, destination, yd, xd):
    height_source = source.shape[0]
    width_source = source.shape[1]
    height_destination = destination.shape[0]
    width_destination = destination.shape[1]
    for y in range(height_source):
        if (y + yd < height_destination):
            for x in range(width_source):
                if(x + xd < width_destination):
                    destination[y + yd, x + xd] = source[y, x]


# create enlared so the border artifacts fall without the  area that should be resized
# values will be extended along the border
def enlarge(map, border, extra):
    height = map.shape[0]
    width = map.shape[1]
    enlarged = np.zeros(( height + border * 2 + extra, width + border * 2 + extra))
    blit(map, enlarged, border, border)

    # vertical borders
    for y in range(0, height):
        l = map[y, 0]
        r = map[y, -1]
        for x in range(0, border):
            enlarged[y, border - x - 1] = l
            enlarged[y, width + border + x] = r
    
    # horizontal borders
    for x in range(0, width):
        t = map[0, x]
        b = map[-1, x]
        for y in range(0, border):
            enlarged[border - y - 1, x] = t
            enlarged[height + border + y, x] = b

    # corners
    tl = map[0, 0]
    tr = map[0, -1]
    bl = map[-1, 0]
    br = map[-1, -1]
    for y in range(0, border):
        for x in range(0, border):
            enlarged[border - y - 1, border - x - 1] = tl
            enlarged[border - y - 1, width + border + x] = tr
            enlarged[height + border + y, border - x - 1] = bl
            enlarged[height + border + y, width + border + x] = br

    return enlarged


# upscales a map of arbitrary size
def upscale(model, map):
    height = map.shape[0]
    width = map.shape[1]

    BORDER = 4
    enlarged = enlarge(map, BORDER, TILESIZE)
    WINDOW = TILESIZE - BORDER * 2

    result = np.zeros((height * 2, width * 2))
    for y in range(0, height, WINDOW):
        for x in range(0, width, WINDOW):
            yt = y + BORDER
            xt = x + BORDER
            part = enlarged[yt:(yt+TILESIZE), xt:(xt+TILESIZE)]
            part = part.reshape(1, TILESIZE * TILESIZE)
            upsampled = model.predict(part)
            upsampled = upsampled.reshape(TILESIZE * 2, TILESIZE * 2)
            window = upsampled[BORDER:BORDER+WINDOW*2,BORDER:BORDER+WINDOW*2]
            blit(window, result, y * 2, x * 2)
    return result


while True:
    model.fit(X_train, Y_train, batch_size=BATCH_SIZE, epochs=EPOCHS, verbose=VERBOSE, validation_split=VALIDATION_SPLIT)
    model.save_weights(WEIGHTS_PATH)

    test_loss, test_acc = model.evaluate(X_test, Y_test)
    print('MSE: ', test_acc)

    ys = model.predict(X_train)
    fileNames = []

    for file in sorted(glob.glob(IMG_PATH +"tiles-training-x/*.png")):
        fileNames.append(os.path.split(file)[1])

    count = 0
    for y in ys:
        yt = Y_train[count]
        dif = np.clip((np.absolute(np.subtract(y, yt)) * 2), 0, 1)

        heightmap.arrayToPng(y, IMG_PATH + "tiles-predicted/{}".format(fileNames[count]))
        heightmap.arrayToPng(dif, IMG_PATH + "tiles-diff/{}".format(fileNames[count]))

        upscaled = None
        if count % 20 == 0:
            for i in range(1, 5):
                
                if i == 1:
                    upscaled = heightmap.pngToArray(IMG_PATH + "upscaled-{}/{}".format(i, fileNames[count]))
                    upscaled = upscaled.reshape( TILESIZE * 2, TILESIZE * 2 )
                else:
                    upscaled = upscale(model, upscaled)
                    array = upscaled.flatten()
                    heightmap.arrayToPng(array, IMG_PATH + "upscaled-{}/{}".format(i, fileNames[count]))

        count = count + 1


    





# count = 0
# for x in X_train:
#     y = model.predict(x)

#     arrayToPng(y, "C:/Projects/heightmap_upscale/data/8-bit/tiles-predicted/{}.png".format(count))



