import face_recognition as face
import cv2 as cv

class HoloLens:
    def __init__(self):
        self.video = cv.VideoCapture(0)
        self.seeing = False

    def __del__(self):
        if self.seeing:
            cv.destroyAllWindows()
        self.video.release()

    def detect_people(self, image):
        numf = face.face_locations(image)
        return len(numf), numf

    def sight(self):
        while True:
            ret, frame  = self.video.read()
            sframe = cv.resize(frame, (0,0),fx=0.25,fy=0.25)
            rframe = sframe[:,:,::-1]
            numpeeps, locations = self.detect_people(rframe);

            for (top,right,bottom,left) in locations:
                top *=4
                right *= 4
                bottom *= 4
                left *= 4

                cv.rectangle(frame, (left, top), (right,bottom), (0,0,255),2)
                font = cv.FONT_HERSHEY_DUPLEX
                cv.putText(frame, "PERSON", (left+6,bottom-6),font,1.0,(255,255,255),1)

                cv.imshow('Wut', frame)
            if cv.waitKey(1) & 0xFF == ord('q'):
                break

h = HoloLens()
h.sight()
