#ifndef _MYIMAGE_
#define _MYIMAGE_ 

#include <opencv2/imgproc/imgproc.hpp>
#include<opencv2/opencv.hpp>
#include <vector>

using namespace cv;
using namespace std;

class ImageSource {
public:
	ImageSource(int webCamera);
	ImageSource();
	Mat downsampled;
	Mat original;
	Mat binary;
	vector<Mat> bwList;
	VideoCapture capture;
	int cameraSrc;
};



#endif
