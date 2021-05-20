from flask import Flask
from flask import Blueprint
from flask import request
# import test as test
# import translate as translator
import ocror as ocror
import detectlang as detect
import os
# os.environ['TF_CPP_MIN_LOG_LEVEL'] = '3'  # or any {'0', '1', '2'}
# import tensorflow as tf
import dl_translate as dlt

app = Flask(__name__)
app.secret_key = b'_5#y2L"F4Q8z\n\xec]/'
mt=dlt.TranslationModel()
lang_code_map=mt.get_lang_code_map()
code_lang_map = {v: k for k, v in lang_code_map.items()}

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
    print('a')
    print(input)
    to=request.args.get('to', '')
    print(to)
    
    # output=translator.translate(input)
    output=mt.translate(input, source=dlt.lang.ENGLISH, target=dlt.lang.CHINESE)

    print(output)
    return output

def langcode(code):
    if code=='zh' or code =='zh-cn' or code=='zh-tw' or code =='zh-Hans':
        return 'zh'
    if code=='ja' or code=='jp':
        return 'ja'
    return code

@app.route("/i2t", methods=['GET', 'POST'])
def to():
    image=request.data
    # print(image)
    text=ocror.recognition(image)
    source=langcode(detect.detectlanguage(text))
    target=langcode(request.args.get('to', 'zh'))
    # ret={'text':text,'language':lang}
    print(text)
    print(source)
    print(target)
    source = code_lang_map.get(source.capitalize(), source)
    target = code_lang_map.get(target.capitalize(), target)
    output=mt.translate(text, source=source, target=target)
    # output=translator.translate(input)
    # output=mt.translate(input, source=dlt.lang.ENGLISH, target=dlt.lang.CHINESE)
    print(output)
    return output

def main(argv):
    print(argv)

# if __name__ == "__main__":
#     tf.compat.v1.logging.set_verbosity(tf.compat.v1.logging.ERROR)
#     tf.app.run()
