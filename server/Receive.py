import os,sys
import subprocess as sub
import struct
from flask import Flask, request, render_template
import base64 as bs
import io
from urllib.parse import unquote
from PIL import Image
app = Flask(__name__)

@app.route('/upload', methods=['POST'])

def upload():
    print('GOT SOMETHING')
    #print(request.get_data())
    #with open('shit.jpeg', 'wb') as f:
#        f.write(request.get_data())
#        f.close()
    print(request.headers)
    print(request.get_data(as_text=True))
    with open('sadhiut.jpeg', 'w') as f:
        f.write(request.get_data(as_text=True))
        f.close()
    #output = None
    #with open('gudstuff.txt', 'r') as f:
    #    output = f.read()
    #return outp
    return 'akfjasdflkjklesaajfslk'

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=8080)
