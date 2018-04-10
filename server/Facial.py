import face_recognition as face
from multiprocessing import Process, Queue, Pool, Manager
import cv2 as cv
import os
from functools import partial
import _pickle as pickle

def create_faces(path, pname):
    new_dict = {}
    for fil in os.listdir(path):
        jod = os.path.join(path, fil)
        #print(jod)
        if os.path.isfile(jod) and ('.jpeg' in fil or '.jpg' in fil):
            #print
            try:
                name = fil[:fil.index('.')]
                img = face.load_image_file(jod)
                enc = face.face_encodings(img)[0]
                new_dict[name] = enc
                print('Processed: '+ jod)
            except:
                print('Exception while processing image, passing '+jod+'...')
    if new_dict:
        try:
            with open(pname+'.txt','wb') as f:
                pickle.dump(new_dict, f)
            print('Successfully dumped face encodings!')
        except:
            print('Error dumping facial encodings!')

def load_faces(path):
    dic_obj = {}
    with open(path, 'rb') as stuff:
        dic_obj = pickle.load(stuff)
    return dic_obj

def compare_encodings(encodings, names, enc):
    match = 'Unknown'
    vals = face.compare_faces(encodings, enc, tolerance=0.5)
    if True in vals:
        match = names[vals.index(True)]
    return match

def multiprocess_comparison(e, n,ne):
    p = Pool(4)
    args = partial(compare_encodings,e,n)
    retval = p.map(args, ne)
    p.close()
    return retval


dick = load_faces('all_faces.txt')
while True:
    enc = []
    try:
        for fil in os.listdir('faces'):
            jod = os.path.join('faces',fil)
            img = face.load_image_file(jod)
            enc.append(face.face_encodings(img)[0])
            os.remove(jod)
        if enc:
            val = multiprocess_comparison(list(dick.values()), list(dick.keys()), enc)
            if val:
                print(val)
    except:
        pass
