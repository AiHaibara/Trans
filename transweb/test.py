import sys
import tensorflow as tf
tf.disable_v2_behavior()
# from tensor2tensor.bin import t2t_decoder
import decoder as t2t_decoder

# DATA_DIR = os.path.expanduser("t2t/data") # This folder contain the data
# TMP_DIR = os.path.expanduser("t2t/tmp")
# TRAIN_DIR = os.path.expanduser("t2t/train") # This folder contain the model
# EXPORT_DIR = os.path.expanduser("t2t/export") # This folder contain the exported model for production
# TRANSLATIONS_DIR = os.path.expanduser("t2t/translation") # This folder contain  all translated sequence
# EVENT_DIR = os.path.expanduser("t2t/event") # Test the BLEU score
# USR_DIR = os.path.expanduser("t2t/user") # This folder contains our data that we want to add
 
# tf.gfile.MakeDirs(DATA_DIR)
# tf.gfile.MakeDirs(TMP_DIR)
# tf.gfile.MakeDirs(TRAIN_DIR)
# tf.gfile.MakeDirs(EXPORT_DIR)
# tf.gfile.MakeDirs(TRANSLATIONS_DIR)
# tf.gfile.MakeDirs(EVENT_DIR)
# tf.gfile.MakeDirs(USR_DIR)
# PROBLEM = "translate_enzh_wmt32k" # We chose a problem translation English to French with 32.768 vocabulary
# MODEL = "transformer" # Our model
# HPARAMS = "transformer_big" # Hyperparameters for the model by default 
#                             # If you have a one gpu, use transformer_big_single_gpu

# train_steps = 300000 # Total number of train steps for all Epochs
# eval_steps = 100 # Number of steps to perform for each evaluation
# batch_size = 4096
# save_checkpoints_steps = 1000
# ALPHA = 0.1
# schedule = "continuous_train_and_eval"
# BEAM_SIZE=1

flags = tf.flags
FLAGS = flags.FLAGS
# flags = tf.flags
# FLAGS = flags.FLAGS
# flags.DEFINE_string('data_dir', 'bbb', 'This is the rate in training')

def translate(input):
    sf = open("input.en", "w+", encoding='utf-8') 
    sf.write(input)
    sf.close()
    FLAGS.data_dir='t2t/data'
    FLAGS.problem='translate_enzh_wmt32k'
    FLAGS.model='transformer'
    FLAGS.hparams_set='transformer_big'
    FLAGS.output_dir='t2t/train'
    FLAGS.decode_hparams='beam_size=1,alpha=0.1'
    FLAGS.decode_from_file='input.en'
    FLAGS.decode_to_file='output.zh'
    #sys.argv=['test.py', '--data_dir=t2t/data', '--problem=translate_enzh_wmt32k', '--model=transformer', '--hparams_set=transformer_big', '--output_dir=t2t/train', '--decode_hparams=beam_size=1,alpha=0.1', '--decode_from_file=input.en', '--decode_to_file=output.zh']
    # print(FLAGS.data_dir)
    t2t_decoder.main(sys.argv)
    f = open("output.zh", encoding='utf-8') 
    output=f.read()
    sf.close()
    return output

def main(argv):
    print(translate("can you speak english?"))

if __name__ == "__main__":
    tf.compat.v1.logging.set_verbosity(tf.compat.v1.logging.ERROR)
    tf.app.run()
