from auth import api_key
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
app.secret_key = '_5#y2LF4Q8z]/'
mt=dlt.TranslationModel()
lang_code_map=mt.get_lang_code_map()
code_lang_map = {v: k for k, v in lang_code_map.items()}

from functools import wraps
from flask import g, request, jsonify, redirect, url_for
def api_key(f):
    @wraps(f)
    def decorator(*args, **kwargs):

        token = None

        if 'x-api-key' in request.headers:
            token = request.headers['x-api-key']

        if not token:
            return jsonify({'message': 'a valid token is missing'})

        try:
            valid=token==app.secret_key
            if not valid:
                return jsonify({'message': 'token is invalid'})
        #  data = jwt.decode(token, app.config[SECRET_KEY])
        #  current_user = Users.query.filter_by(public_id=data['public_id']).first()
        except:
            return jsonify({'message': 'token is invalid'})

        return f(*args, **kwargs)
    return decorator

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
@api_key
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
