import os,sys
import subprocess as sub
import struct
from flask import Flask, request, render_template
import base64 as bs
from urllib.parse import unquote
app = Flask(__name__)

@app.route('/upload', methods=['POST'])

def upload():
    print('Something was received...')
    string = unquote(request.get_data(as_text=True))[5:]
    img = bs.b64decode(string)
    with open('tmp.jpeg','wb') as f:
        f.write(img)
    out = sub.check_output(['./detect', 'tmp.jpeg']).decode('ASCII').replace('\n','')
    os.remove('tmp.jpeg')
    return out.replace('[ INFO:0] Initialize OpenCL runtime...', '')

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8080)
