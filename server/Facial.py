

import face_recognition as face
from multiprocessing import Process, Queue, Pool
import cv2 as cv
import os
from functools import partial
import _pickle as pickle

'''
Creates a pickled dictionary list with names and face encodings of people.
-------------------------------------------------------------------------
Arguments:
    -path <== Path to directory with people's faces.
    -pname <== Name the user wishes to call the file.
'''
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

'''
Load the faces of a pickled dictionary list with people's face encodings.
------------------------------------------------------------------------
Arguments:
    -path <== Path to file.
'''
def load_faces(path):
    dic_obj = {}
    with open(path, 'rb') as stuff:
        dic_obj = pickle.load(stuff)
    return dic_obj

'''
Compare a facial encoding with a list of other facial encodings and retrieve a possible name for the match.
-----------------------------------------------------------------------------------------------------------
Arguments:
    -encodings <== List of all known facial encodings.
    -names <== Names corresponding to the facial encodings.
    -enc <== Facial encoding to be compared.
'''
def compare_encodings(encodings, names, enc):
    match = 'Unknown'
    vals = face.compare_faces(encodings, enc, tolerance=0.5)
    if True in vals:
        match = names[vals.index(True)]
    return match

'''
Function to multiprocess the compare_encodings function.
--------------------------------------------------------
Arguments:
    -e <== List of all known facial encodings.
    -n <== Names corresponding to the facial encodings.
    -ne <== Facial encoding to be compared.
'''
def multiprocess_comparison(e, n,ne):
    p = Pool(4)
    args = partial(compare_encodings,e,n)
    retval = p.map(args, ne)
    p.close()
    return retval


'''
Main loop.
Will need to be changed if used with other projects, since it's written for this project only.
All other functions should be able to work with other projects as is, though.
'''
file_dict = load_faces('all_faces.txt')
while True:
    enc = []
    try:
        for fil in os.listdir('faces'):
            jod = os.path.join('faces',fil)
            img = face.load_image_file(jod)
            enc.append(face.face_encodings(img)[0])
            os.remove(jod)
        if enc:
            val = multiprocess_comparison(list(file_dict.values()), list(file_dict.keys()), enc)
            if val:
                print(val)
    except:
        pass
