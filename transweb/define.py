PROBLEM = "translate_enzh_wmt32k" # We chose a problem translation English to French with 32.768 vocabulary
MODEL = "transformer" # Our model
HPARAMS = "transformer_base" # Hyperparameters for the model by default 
                            # If you have a one gpu, use transformer_big_single_gpu

import tensorflow as tf
import os

DATA_DIR = os.path.expanduser("t2t/data") # This folder contain the data
TMP_DIR = os.path.expanduser("t2t/tmp")
TRAIN_DIR = os.path.expanduser("t2t/train") # This folder contain the model
EXPORT_DIR = os.path.expanduser("t2t/export") # This folder contain the exported model for production
TRANSLATIONS_DIR = os.path.expanduser("t2t/translation") # This folder contain  all translated sequence
EVENT_DIR = os.path.expanduser("t2t/event") # Test the BLEU score
USR_DIR = os.path.expanduser("t2t/user") # This folder contains our data that we want to add
 
tf.io.gfile.makedirs(DATA_DIR)
tf.io.gfile.makedirs(TMP_DIR)
tf.io.gfile.makedirs(TRAIN_DIR)
tf.io.gfile.makedirs(EXPORT_DIR)
tf.io.gfile.makedirs(TRANSLATIONS_DIR)
tf.io.gfile.makedirs(EVENT_DIR)
tf.io.gfile.makedirs(USR_DIR)