import os
import subprocess as sub
from flask import Flask, request, render_template
app = Flask(__name__)

@app.route('/upload', methods=['POST'])

def upload():
    imagefile = request.files.get('file', '')
    #print(request.files)
    imagefile.save('tmp.jpg')
    out = sub.check_output(['./detect', 'tmp.jpg'])
    os.remove('tmp.jpg')
    return out

if __name__ == '__main__':
    app.run(host='0.0.0.0', port=80)
