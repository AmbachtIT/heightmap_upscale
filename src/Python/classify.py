import tensorflow as tf
import numpy as np
import heightmap
import model
from tensorflow import keras
import glob
import os
import util


# Parameters
DATA_PATH = "C:/Projects/heightmap_upscale/data/"
IMG_PATH = DATA_PATH + "8-bit/"
WEIGHTS_PATH = DATA_PATH + "weights.h5"

model = models.createUpscaleModel()




for i in range(1, 4):







