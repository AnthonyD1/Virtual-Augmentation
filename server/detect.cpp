/**
#######################################
#########Code made by Paul D.##########
#######################################
**/

//These libraries are for the face detection
#include <opencv2/objdetect.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/video.hpp>
#include <opencv2/videoio.hpp>
//Used for handling file logic, vectors, etc.
#include <iostream>
#include <iomanip>
#include <memory>
#include <stdexcept>
#include <string>
#include <array>
#include <future>
#include <fstream>

/**
Some people will complain about using the
namespaces for both std and cv as being 
bad practice. But for simplicity, I did.
**/

using namespace std;
using namespace cv;

/**
This is the primary class for detecting people's faces, as 
well as any other feature detection that we wanted to do. 
Plus, the class also provides drawing functions for all the 
features. 
**/
class People{
  //Our main face classifier.
  CascadeClassifier headDetector;
public:
  People(){
    //Loading the default haarcascade classifier for frontal based detection.
    headDetector.load("haarcascade_frontalface_default.xml");
  }
  /**
  Detect people's faces, and return a vector of everybody's face locations.
  ----------------------------------------------------------------------------
  Arguments:
    -InputArray img <== Mat image from a OpenCV capture object.
  **/
  vector<Rect> detectHead(InputArray img){
    vector<Rect> temp;
    /**
      Main detection is used here:
      First argument is the frame image that we want to analyze.
      Second, the rectangle struct vector that will store the faces found.
      Third, scaling that will be used to resize the image for analyzation.
      Fourth, the minimum amount of neighbors needed inside a detected face pattern.
        -This helps eliminate false positives.
      Fifth, use the haarcascade detection.
    **/
    headDetector.detectMultiScale(img, temp, 1.1, 8, 1);
    return temp;
  }

  /**
  Given a vector of rectangle points, it will draw a custom design over each person's face.
  ----------------------------------------------------------------------------------------
  Arguments:
    -Mat &frame <== Reference to image frame captured from OpenCV Capture object.
    -vector<Rect> &r <== Reference to a vector of rectangle coordinates.
  **/
  void drawHeads(Mat & frame, vector<Rect> &r){
    //Remember that rectangle struct vector? Let's iterate through all the found faces
    //and draw a rectangle around them.
    for(int head = 0; head < r.size(); head++){
      rectangle(frame, r[head].tl(), r[head].br(), Scalar(0,255,0), 2,8,0);
      Point tl = r[head].tl();
      Point br = r[head].br();
      Point top(((br.x - tl.x)/2)+tl.x, tl.y);
      Point ntop(top.x, top.y-50);
      Point bottom(((br.x - tl.x)/2)+tl.x, br.y);
      Point nbottom(bottom.x, bottom.y+50);
      Point left(tl.x, ((tl.y-br.y)/2)+br.y);
      Point nleft(left.x-50, left.y);
      Point right(br.x, ((tl.y-br.y)/2)+br.y);
      Point nright(right.x+50, right.y);
      line(frame, top, ntop, Scalar(0,255,0), 2);
      line(frame, bottom, nbottom, Scalar(0,255,0), 2);
      line(frame, left, nleft, Scalar(0,255,0), 2);
      line(frame, right, nright, Scalar(0,255,0),2);
    }
  }
  
  /**
  Returns how many people's faces were found, from the length of an image's rectangle structs.
  --------------------------------------------------------------------------------------------
  Arguments:
    -vector<Rect> &r <== Reference to a vector of rectangle coordinates.
  **/
  int headCount(vector<Rect> &r){
    //Simply get number of people that the program found in the image frame.
    return r.size();
  }
  
  /**
  Crops out peoples faces, and puts them in a vector of matrices so they can be displayed individually.
  ----------------------------------------------------------------------------------------------------
  Arguments:
    -Mat &of <== Reference to image frame captured from OpenCV Capture object.
    -vector<Rect> &r <== Reference to a vector of rectangle coordinates.
  **/
  vector<Mat> headImages(Mat &of, vector<Rect> &faces){
    vector<Mat> fimages;
    for(int i = 0; i < faces.size(); ++i){
      fimages.push_back(of(faces[i]));
    }
    return fimages;
  }
};
//END OF PEOPLE CLASS

/**
Function to execute a system command, and return it's output.
------------------------------------------------------------
Arguments:
  -const char* cmd <== Command to be run. 
**/
string exec(const char* cmd) {
    std::array<char, 128> buffer;
    std::string result;
    std::shared_ptr<FILE> pipe(popen(cmd, "r"), pclose);
    if (!pipe) throw std::runtime_error("popen() failed!");
    while (!feof(pipe.get())) {
        if (fgets(buffer.data(), 128, pipe.get()) != nullptr)
            result += buffer.data();
    }
    return result;
}

/**
Perform detection on an image.
------------------------------
 Arguments:
    -VideoCapture & cap <== Reference to a video capture object. 
    -People &people <== Reference to a People object.
    -char *path <== Path to image file.
    -int show <== Number representing levels of processing.
        -- "0" <== Only number of faces.
        -- "1" <== Number of faces, and picture is displayed with graphics drawn around faces.
        -- "2" <== Number of faces & coordinates of faces, plus picture is displayed with graphics drawn around faces.
**/
int pictureDetect(VideoCapture & cap, People &people, char *path, int show){
  cap.open(path);
  Mat tmp;
  if(!cap.isOpened()){
    cout << "Couldn't open image..." << endl;
    exit(0);
  }
  cap >> tmp;
  vector<Rect> fheads;
  if(show == 1){
    Mat img;
    cvtColor(tmp, img, CV_BGR2GRAY);
    equalizeHist(img, img);
    fheads = people.detectHead(img);
    people.drawHeads(tmp, fheads);
    imshow("Feed", tmp);
    waitKey(1);
    cin.get();
  }else{
    cvtColor(tmp, tmp, CV_BGR2GRAY);
    equalizeHist(tmp, tmp);
    fheads = people.detectHead(tmp);
  }
  if(show >= 1){
    for(int i = 0; i < fheads.size(); ++i){
      cout << fheads[i].tl() << ", " << fheads[i].br() << endl;
    }
  }
  //cout << "Head count: " << people.headCount(fheads) << endl;
  int numHeads = people.headCount(fheads);
  //people.drawHeads(image, fheads);
  return numHeads;
}

/**
  Write a matrices of people's faces to individual files and resize them to 650px by 650px for processing. 
  --------------------------------------------------------------------------------------------------------
  Arguments:
    -Mat r <== Image frame.
    -vector<Rect> heads <== Vector of rentangle coordinates of people's faces.
**/
void writeToFile(Mat r, vector<Rect> heads){
    #pragma omp parallel for
    for(int i = 0; i < heads.size(); i++){
      Mat lol;
      resize(r(heads[i]), lol, Size(650,650));
      char nut[100];
      sprintf(nut, "faces/process%d.jpeg", i);
      imwrite(nut, lol);
    }
}

/**
Write the headcount of people seen to a file for processing.
-------------------------------------------------------------
Arguments:
  -int fin <== Number of people seen in an image. 
**/
void writePeopleSeen(int fin){
  ofstream myfile;
  myfile.open("Seen.txt");
  myfile << fin;
  myfile.close();
}

/**
  Perform real-time face detection on a system's webcam.
  --------------------------------------------------------
  Arguments:
    -VideoCapture & cap <== Reference to a video capture object. 
    -People &people <== Reference to a People object.
    -bool demo <== Set to TRUE if you want facial features to be written to a file, FALSE if you just want to do normal processing.
**/
void cameraDetect(VideoCapture & cap, People &people, bool demo=false){
  Mat img;
  cap.open(0);
  namedWindow("Feed", WINDOW_NORMAL);
  resizeWindow("Feed", 1500,1200);
  if(!cap.isOpened()){
    cout << "Couldn't find the webcam..." << endl;
    exit(0);
  }
  int counter = 0;
  while(true){
    cap >> img;
    Mat tmp;
    //Create a copy of the image to convert to grayscale and equalize the histograms.
    cvtColor(img, tmp, CV_BGR2GRAY);
    //ied equalized and grayscaled image.
    vector<Rect> fheads = people.detectHead(tmp);
    if(demo){
      writePeopleSeen(fheads.size());
    }
    //Draw the found faces on the original image, if any.
    people.drawHeads(img,fheads);
    //Display the colored image with the drawn faces.
    if(demo){
      if(counter > 20){
        counter = 0;
        std::future<void> result(std::async(writeToFile, img, fheads));
      }
      else{
        counter++;
      }
    }
    resize(img,img,Size(4000,2250));
    imshow("Feed", img);
    const char key = (char) waitKey(1);
    if(key == 27 || key == 'q'){
      cout << "End..." << endl;
      break;
    }
  }
}

//Print out the usage of the program for the user
void usage(){
  cout << "./detect <== Uses default camera to perform real-time face detection" << endl;
  cout << "./detect <picture to detect faces on> <== Return a count of people's faces in an image" << endl;
  cout << "./detect <picture to detect faces on> 1 <== Display image with graphics on people's faces, and number of faces is outputted" << endl;
  cout << "./detect <picture to detect faces on> 2 <== Display image with graphics, top left & bottom right coordinates and number of faces outputted" << endl;
}

int main(int argc, char** argv){
  VideoCapture cap;
  People people;
  int out = 0;
  if (argc > 1) && (argv[2] == "--help"){
    usage();
    return -1;
  }
  switch(argc){
    case 1:
      cameraDetect(cap, people, false);
      break;
    case 2:
      out = pictureDetect(cap, people, argv[1], 0);
      break;
    case 3:
      out = pictureDetect(cap, people, argv[1], atoi(argv[2]));
      break;
    default:
      usage();
      return -1;
  }
  cout << out << endl;
  return 0;
}
