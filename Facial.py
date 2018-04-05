import face_recognition as face
from multiprocessing import Process, Queue, Pool, Manager
from time import sleep
import cv2 as cv
import itertools
import pickle

def load_face_images():
    dic_obj = {}
    with open('Backup.txt') as stuff:
        dic_obj = pickle.load(stuff)
    return dic_obj

faces = load_face_images()
encodings = faces.values()
names = faces.keys()

def detect_faces(image):
    loc = face.face_locations(image)

def recognize_faces(enc):
    match = None
    #print('Multiprocess Started!')
    vals = face.compare_faces(encodings, enc, tolerance=0.55)
    if True in vals:
        match = names[vals.index(True)]
    print('Name: '+str(match))
    #qseen.append(match)
    #qseen += 1

def get_encodings(image):
    return pool.map(face.face_encodings, [image])

def detect_people(image):
    found, _ = hog.detectMultiScale(image,  winStride=(8,8), padding=(32,32), scale=1.05)
    return found

def draw_detections( img, rects, thickness = 1):
    #print rects
    for x, y, w, h in rects:
        # the HOG detector returns slightly larger rectangles than the real objects.
        # so we slightly shrink the rectangles to get a nicer output.
        pad_w, pad_h = int(0.15*w), int(0.05*h)
        cv.rectangle(img, (x+pad_w, y+pad_h), (x+w-pad_w, y+h-pad_h), (0, 255, 0), thickness)

def resize_frame(f, foo):
    return f[:,:,::-1]

def sight():
    pframe = False

    while True:
        ret, frame  = video.read()
        if pframe:

            sframe = cv.resize(frame, (0,0), fx=.25, fy=.25)
        #mframe = cv.resize(frame, (0,0),fx=.5, fy=.5)
            pool.map(resize_frame, [frame])

            #Process(target=detect_faces, args=(rframe,)).start()

            sdf = get_encodings(retframe)
            retframe = None
        #q = Queue()
            print '\nFaces:'
            print '-'*50
            if sdf:
                #pool.map(recognize_faces, sdf)
                print(sdf)
        '''for (top,right,bottom,left) in locations:
                top *=4
                right *= 4
                bottom *= 4
                left *= 4
                    cv.rectangle(frame, (left, top), (right,bottom), (0,0,255),2)
                font = cv.FONT_HERSHEY_DUPLEX
                cv.putText(frame, "PERSON", (left+6,bottom-6),font,1.0,(255,255,255),1)'''
        pframe = not pframe
        cv.imshow('Wut', frame)
        if cv.waitKey(1) & 0xFF == ord('q'):
            break


pool = Pool(3)
video = cv.VideoCapture(0)
hog = cv.HOGDescriptor()
hog.setSVMDetector(cv.HOGDescriptor_getDefaultPeopleDetector())
locs = None

sight()
