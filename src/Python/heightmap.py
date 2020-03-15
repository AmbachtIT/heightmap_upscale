import tensorflow as tf
import numpy as np
import png as png
import math
from tensorflow import keras
import glob
import os

TILESIZE = 128

def pngToArray(path):
    map = loadHeightmap(path, np.uint8)
    map = map.flatten().astype('float32')
    map /= 255
    return map

def arrayToPng(map, path):
    dim = int(math.sqrt(map.shape[0]))
    map = np.clip(map.reshape(dim, dim), 0, 1)
    map *= 255
    map = map.astype(np.int8)
    png.from_array(map, mode="L").save(path)
    return

def loadHeightmaps(path):
    list = []
    for file in sorted(glob.glob(path + "*.png")):
        map = pngToArray(file)
        list.append(map)    
    return np.stack(list)

def loadHeightmap(path, type):
    pngData = png.Reader(filename = path).asDirect()
    pngInfo = pngData[3]
    pngRows = list(pngData[2])
    size = pngInfo['size']
    width = size[0]
    height = size[1]
    data = np.zeros((height, width), dtype = type)
    step = 3
    if pngInfo['greyscale']:
        step = 1

    for y in range(height):
        row = pngRows[y]
        for x in range(width):
            grey = row[x * step]
            data[y, x] = grey    
    return data

def downSampleHeightmap(data, type, times):
    width = int(data.shape[1] / times)
    height = int(data.shape[0] / times)
    copy = np.zeros((width, height), dtype = type)
    for y in range(height):
        y1 = y * times
        y2 = y1 + times
        for x in range(width):
            x1 = x * times
            x2 = x1 + times
            pixel = data[y1:y2, x1:x2]
            copy[y, x] = np.average(pixel)

    return copy

def run(outputPath):
    for originalPath in glob.glob("C:/Projects/heightmap_upscale/data/8-bit/originals/*.png"):
        fileName = os.path.split(originalPath)[1]
        print("processing: ", fileName)

        type = np.uint8
        original = loadHeightmap(originalPath, type)
        width = original.shape[1]
        height = original.shape[0]

        prefix = fileName.split('.')[0]

        for y in range(int(height / TILESIZE)):
            y1 = y * TILESIZE
            y2 = y1 + TILESIZE
            for x in range(int(width / TILESIZE)):
                x1 = x * TILESIZE
                x2 = x1 + TILESIZE
                tile = original[y1:y2, x1:x2]
                tileName = "{}-{}-{}.png".format(prefix, x, y)
                if hash(tileName) % 10 >= 8:
                    folder = "tiles-test"
                else:
                    folder = "tiles-training"
                png.from_array(tile, mode="L").save(outputPath + folder + "-y/" + tileName)

                x = downSampleHeightmap(tile, type, 2)
                png.from_array(x, mode="L").save(outputPath + folder + "-x/" + tileName)
    return

# run("D:/Data/heightmap_upscale/8-bit/")




#     #png.from_array(original, mode="L").save(outputPath + fileName) # mode='L;16' for greyscale

#     print("Resolution: ", width, ", ", height)
#     print()





