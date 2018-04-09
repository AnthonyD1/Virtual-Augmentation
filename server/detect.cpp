#include <opencv2/objdetect.hpp>
#include <opencv2/highgui.hpp>
#include <opencv2/imgproc.hpp>
#include <opencv2/video.hpp>
#include <opencv2/videoio.hpp>
#include <iostream>
#include <iomanip>
#include <memory>
#include <stdexcept>
#include <string>
#include <array>
#include <future>

using namespace std;
using namespace cv;

class People{
  CascadeClassifier headDetector;
public:
  People(){
    //Loading the default haarcascade classifier for frontal based detection.
    headDetector.load("haarcascade_frontalface_default.xml");
  }

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

  void drawHeads(Mat & frame, vector<Rect> &r){
    //Remember that rectangle struct vector? Let's iterate through all the found faces
    //and draw a rectangle around them.
    for(int head = 0; head < r.size(); head++){
      rectangle(frame, r[head].tl(), r[head].br(), Scalar(0,0,255), 2,8,0);
    }
  }
  int headCount(vector<Rect> &r){
    //Simply get number of people that the program found in the image frame.
    return r.size();
  }
  vector<Mat> headImages(Mat &of, vector<Rect> &faces){
    vector<Mat> fimages;
    for(int i = 0; i < faces.size(); ++i){
      fimages.push_back(of(faces[i]));
    }
    return fimages;
  }
};

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
  if(show > 1){
    for(int i = 0; i < fheads.size(); ++i){
      cout << fheads[i].tl() << ", " << fheads[i].br() << endl;
    }
  }
  //cout << "Head count: " << people.headCount(fheads) << endl;
  int numHeads = people.headCount(fheads);
  //people.drawHeads(image, fheads);
  return numHeads;
}

void writeToFile(Mat r){
    imwrite("process.jpg", r);
}

void cameraDetect(VideoCapture & cap, People &people, bool demo=false){
  Mat img;
  cap.open(0);
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

    //Draw the found faces on the original image, if any.
    people.drawHeads(img,fheads);
    //Display the colored image with the drawn faces.
    if(demo){
      if(counter > 5){
        counter = 0;
        std::future<void> result(std::async(writeToFile, img));
      }
      else{
        counter++;
      }
    }
    imshow("Feed", img);
    const char key = (char) waitKey(1);
    if(key == 27 || key == 'q'){
      cout << "End..." << endl;
      break;
    }
  }
}

void usage(){
  cout << "./detect <picture to detect faces on> <'true' to show frame, 'false' to just use console>" << endl;
}
/**
int main(int argc, char** argv){
  VideoCapture cap;
  People people;
  Mat tmp;
  Mat img;
  cap.open("Paul.jpeg");
  if(!cap.isOpened()){
    cout << "Failed!" << endl;
    return -1;
  }
  cap >> tmp;
  cvtColor(tmp, img, CV_BGR2GRAY)
  equalizeHist(img, img);
  vector<Rect> headshots = people.detectHead(img);
  vector<Mat> peeps = people.headImages(tmp,headshots);
  for(int i = 0; i < peeps.size(); i++){
      imshow("Feed", peeps[i]);
  }
  waitKey(1);
  cin.get();
  return 0;

}**/
int main(int argc, char** argv){
  VideoCapture cap;
  People people;
  int out = 0;
  switch(argc){
    case 1:
      cameraDetect(cap, people, true);
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
