from flask import Flask
from flask import request
# import test as test
import translate as translator
import ocror as ocror
import detectlang as detect
import os
os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'  # or any {'0', '1', '2'}
import tensorflow as tf

app = Flask(__name__)

@app.route("/")
def hello():
    return "Hello, World!"

@app.route("/ocr", methods=['GET', 'POST'])
def ocr():
    # print(request.data)
    # print(request.form)
    # print(request.json)
    # print(request.values)
    # print(request.files) 
    image=request.data
    # print(image)
    text=ocror.recognition(image)
    lang=detect.detectlanguage(text)
    ret={'text':text,'language':lang}
    print(ret)
    return ret

@app.route("/trans", methods=['GET', 'POST'])
def trans():
    input=request.args.get('input', '')
    print(input)
    output=translator.translate(input)
    print(output)
    return output

def main(argv):
    print(argv)

# if __name__ == "__main__":
#     tf.compat.v1.logging.set_verbosity(tf.compat.v1.logging.ERROR)
#     tf.app.run()
