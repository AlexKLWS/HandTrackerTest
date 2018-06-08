#ifndef _HAND_GESTURE_
#define _HAND_GESTURE_ 

#include <opencv2/imgproc/imgproc.hpp>
#include<opencv2/opencv.hpp>
#include <vector>
#include <string>
#include "main.hpp"
#include "imageSource.hpp"

using namespace cv;
using namespace std;

class HandGesture {
public:
	ImageSource m;
	HandGesture();
	vector<vector<Point> > contours;
	vector<vector<int> >hullIndex;
	vector<vector<Point> >hullPoint;
	vector<vector<Vec4i> > defects;
	vector <Point> fingerTips;
	Rect rect;
	void printGestureInfo(Mat src);
	int cIdx;
	int frameNumber;
	int mostFrequentFingerNumber;
	int numberOfDefects;
	Rect bRect;
	double bRect_width;
	double bRect_height;
	bool isHand;
	bool detectIfHand();
	void initVectors();
	void getFingerNumber(ImageSource *m);
	void eleminateDefects();
	void getFingertips(ImageSource *m);
	void drawFingertips(ImageSource *m);
private:
	string bool2string(bool tf);
	int fontFace;
	int prevNrFingerTips;
	void checkForOneFinger(ImageSource *m);
	float getAngle(Point s, Point f, Point e);
	vector<int> fingerNumbers;
	void analyzeContours();
	string intToString(int number);
	void computeFingerNumber();
	void addNumberToImg(ImageSource *m);
	vector<int> numbers2Display;
	void addFingerNumberToVector();
	Scalar numberColor;
	int nrNoFinger;
	float distanceP2P(Point a, Point b);
	void removeRedundantEndPoints(vector<Vec4i> newDefects);
	void removeRedundantFingerTips();
};

#endif
