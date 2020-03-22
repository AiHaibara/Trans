from flask import Flask
from flask import request
import tensorflow as tf
import test as test

app = Flask(__name__)

@app.route("/")
def hello():
    return "Hels slo, World!"

@app.route("/trans", methods=['GET', 'POST'])
def trans():
    input=request.args.get('input', '')
    print(input)
    return test.translate(input)

def main(argv):
    print(argv)

if __name__ == "__main__":
    tf.logging.set_verbosity(tf.logging.INFO)
    tf.app.run()
