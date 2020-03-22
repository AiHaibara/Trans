#Config
import define as define
from tensor2tensor import models
from tensor2tensor import problems
from tensor2tensor.layers import common_layers
from tensor2tensor.utils import trainer_lib
from tensor2tensor.utils import t2t_model
from tensor2tensor.utils import registry
from tensor2tensor.utils import metrics
import numpy as np
import os
import tensorflow as tf
import codecs
import sys, traceback

#After training the model, re-run the environment but run this code in first, then predict.
tfe = tf.contrib.eager
tfe.enable_eager_execution()
Modes = tf.estimator.ModeKeys

enzh_problem = problems.problem(define.PROBLEM)

# Copy the vocab file locally so we can encode inputs and decode model outputs
vocab_name = "vocab.translate_enzh_wmt32k.32768.subwords"
vocab_file = os.path.join(define.DATA_DIR, vocab_name)

# Get the encoders from the problem
encoders = enzh_problem.feature_encoders(define.DATA_DIR)

ckpt_path = tf.train.latest_checkpoint(os.path.join(define.TRAIN_DIR))

hparams = trainer_lib.create_hparams(define.HPARAMS, data_dir=define.DATA_DIR, problem_name=define.PROBLEM)
translate_model = registry.model(define.MODEL)(hparams, Modes.PREDICT)
print(ckpt_path)

def translate(inputs):
  try:
    encoded_inputs = encode(inputs)
    with tfe.restore_variables_on_create(ckpt_path):
      model_output = translate_model.infer(encoded_inputs)["outputs"]
    return decode(model_output)
  except Exception:
    # print("Unexpected error:", sys.exc_info()[0])
    print("Unexpected error:", sys.exc_info())
    exc_type, exc_value, exc_traceback=sys.exc_info()
    traceback.print_tb(exc_traceback, None, file=sys.stdout)
    return 'None'
 
def encode(input_str, output_str=None):
  """Input str to features dict, ready for inference"""
  inputs = encoders["inputs"].encode(input_str) + [1]  # add EOS id
  batch_inputs = tf.reshape(inputs, [1, -1, 1])  # Make it 3D.
  return {"inputs": batch_inputs}

def decode(integers):
  """List of ints to str"""
  integers = list(np.squeeze(integers))
  if 1 in integers:
    integers = integers[:integers.index(1)]
  integers=np.squeeze(integers)
  # print('type')
  # print(type(integers).__name__)
  # if type(integers).__name__!='ndarray':
  # if len(integers)==1:
  integers=np.append(integers, 3)
  # print('datas')
  # print(integers)
  return encoders["targets"].decode(integers)

# inputs = "the animal didn't cross the river because it was too tired"
# outputs = translate(inputs)                          

# print("Inputs: %s" % inputs)
# print("Outputs: %s" % outputs)

# file_input = codecs.open("outputs.zh","w+","utf-8")
# file_input.write(outputs)
# file_input.close()