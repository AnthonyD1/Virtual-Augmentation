import multiprocessing as mp
import cv2 as cv
import itertools
import face_recognition as fr
import pickle
import sys
from PIL import Image

def load_face_images():
    dic_obj = {}
    with open('Backup.txt') as stuff:
        dic_obj = pickle.load(stuff)
    return dic_obj

#faces = load_face_images()
encodings = []
names = []

def draw_locations(frame, locations, names):
    for (top,right,bottom,left), name in zip(locations, names):
            top *=4
            right *= 4
            bottom *= 4
            left *= 4
            cv.rectangle(frame, (left, top), (right,bottom), (0,0,255),2)
            font = cv.FONT_HERSHEY_DUPLEX
            cv.putText(frame, name, (left+6,bottom-6),font,1.0,(255,255,255),1)
    return frame

def comparison(emult):
    match = 'DooD'
    vals = fr.compare_faces(encodings, emult, tolerance=0.55)
    if True in vals:
        match = names[vals.index(True)]
    return match

def recognize_faces(image, locs):
    encs = fr.face_encodings(image, locs)
    names = []
    for enc in encs:
        names.append(comparison(enc))
    return names

def image_resize(image):
    sframe = cv.resize(image, (0,0), fx=.25, fy=.25)
    rframe = sframe[:,:,::-1]
    return rframe

def detect(rframe):
    #rframe = image_resize(image)
    loc = fr.face_locations(rframe)
    names = recognize_faces(rframe, loc)
    s_queue.put(names)

def detect_GUI(image):
    rframe = image_resize(image)
    loc = fr.face_locations(rframe)
    names = recognize_faces(rframe, loc)
    #names = itertools.repeat('Detected', len(loc))
    dframe = draw_locations(image, loc, names)
    s_queue.put(dframe)

def display_frames():
    while True:
        if not s_queue.empty():
            cv.imshow('Video', s_queue.get())
        if cv.waitKey(1) & 0xff == ord('q'):
            sys.exit(0)
            break

def picture_names(path):
    im = fr.load_image_file(path)
    pool.map(detect, [im])
    return s_queue.get()

def picture_recognize(path):
    im = fr.load_image_file(path)
    pool.map(detect_GUI, [im])
    return s_queue.get()

def main_thread(cam):
    mp.Process(target=display_frames).start()
    while True:
        ret, nframe = cam.read()
        print(nframe)
        pool.map(detect_GUI, [nframe])


s_queue = mp.Queue()
pool = mp.Pool(processes=4)
#fin = picture_names('../victims/Angela.jpeg')
#print(fin)
#pool2 = mp.Pool(processes=2)
#pool.map
#pool2 = mp.Pool(processes=3)
video = cv.VideoCapture(0)
main_thread(video)
