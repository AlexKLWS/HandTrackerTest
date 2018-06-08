#include <opencv2/imgproc/imgproc.hpp>
#include <opencv2/opencv.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <stdio.h>
#include <stdlib.h>
#include <iostream>
#include <string>
#include "imageSource.hpp"
#include "roi.hpp"
#include "handGesture.hpp"
#include <vector>
#include <cmath>
#include "main.hpp"

using namespace cv;
using namespace std;

struct PixelData
{
	PixelData(uchar r, uchar g, uchar b) : r(r), g(g), b(b) {}
	uchar r, g, b;
};

enum TrackerState
{
	NONE,
	AWAITNG_PALM,
	GETTING_COLOR_SAMPLE,
	TRACKING
};

/* Global Variables  */
int _fontFace = FONT_HERSHEY_PLAIN;
int _squareLength;
int avgColor[NSAMPLES][3];
int c_lower[NSAMPLES][3];
int c_upper[NSAMPLES][3];
int avgBGR[3];
int nrOfDefects;
struct BoundingDimensions { int w; int h; } boundingDim;
Mat edges;
vector <ColorSampleROI> colorSampleRegions;
vector <KalmanFilter> kf;
vector <Mat_<float>> measurement;

ImageSource _imageSource(0);
TrackerState _currentState;
HandGesture _handGesture;

/* end global variables */

int getMedian(vector<int> val) {
	int median;
	size_t size = val.size();
	sort(val.begin(), val.end());
	if (size % 2 == 0) {
		median = val[size / 2 - 1];
	}
	else {
		median = val[size / 2];
	}
	return median;
}


void getAvgColor(ColorSampleROI roi, int avg[3]) {
	Mat r;
	roi.roi_ptr.copyTo(r);
	vector<int>hm;
	vector<int>sm;
	vector<int>lm;
	// generate vectors
	for (int i = 2; i < r.rows - 2; i++) {
		for (int j = 2; j < r.cols - 2; j++) {
			hm.push_back(r.data[r.channels()*(r.cols*i + j) + 0]);
			sm.push_back(r.data[r.channels()*(r.cols*i + j) + 1]);
			lm.push_back(r.data[r.channels()*(r.cols*i + j) + 2]);
		}
	}
	avg[0] = getMedian(hm);
	avg[1] = getMedian(sm);
	avg[2] = getMedian(lm);
}

void normalizeColors() {
	// copy all boundaries read from trackbar
	// to all of the different boundries
	for (int i = 1; i < NSAMPLES; i++) {
		for (int j = 0; j < 3; j++) {
			c_lower[i][j] = c_lower[0][j];
			c_upper[i][j] = c_upper[0][j];
		}
	}
	// normalize all boundries so that 
	// threshold is whithin 0-255
	for (int i = 0; i < NSAMPLES; i++) {
		if ((avgColor[i][0] - c_lower[i][0]) < 0) {
			c_lower[i][0] = avgColor[i][0];
		}if ((avgColor[i][1] - c_lower[i][1]) < 0) {
			c_lower[i][1] = avgColor[i][1];
		}if ((avgColor[i][2] - c_lower[i][2]) < 0) {
			c_lower[i][2] = avgColor[i][2];
		}if ((avgColor[i][0] + c_upper[i][0]) > 255) {
			c_upper[i][0] = 255 - avgColor[i][0];
		}if ((avgColor[i][1] + c_upper[i][1]) > 255) {
			c_upper[i][1] = 255 - avgColor[i][1];
		}if ((avgColor[i][2] + c_upper[i][2]) > 255) {
			c_upper[i][2] = 255 - avgColor[i][2];
		}
	}
}

void produceBinaries(ImageSource *m) {
	Scalar lowerBound;
	Scalar upperBound;
	for (int i = 0; i < NSAMPLES; i++) {
		normalizeColors();
		lowerBound = Scalar(avgColor[i][0] - c_lower[i][0], avgColor[i][1] - c_lower[i][1], avgColor[i][2] - c_lower[i][2]);
		upperBound = Scalar(avgColor[i][0] + c_upper[i][0], avgColor[i][1] + c_upper[i][1], avgColor[i][2] + c_upper[i][2]);
		m->bwList.push_back(Mat(m->downsampled.rows, m->downsampled.cols, CV_8U));
		inRange(m->downsampled, lowerBound, upperBound, m->bwList[i]);
	}
	m->bwList[0].copyTo(m->binary);
	for (int i = 1; i < NSAMPLES; i++) {
		m->binary += m->bwList[i];
	}
	medianBlur(m->binary, m->binary, 7);
}

int findBiggestContour(vector<vector<Point> > contours) {
	int indexOfBiggestContour = -1;
	int sizeOfBiggestContour = 0;
	for (int i = 0; i < contours.size(); i++) {
		if (contours[i].size() > sizeOfBiggestContour) {
			sizeOfBiggestContour = contours[i].size();
			indexOfBiggestContour = i;
		}
	}
	return indexOfBiggestContour;
}

void myDrawContours(ImageSource *m, HandGesture *hg) {
	drawContours(m->original, hg->hullPoint, hg->cIdx, cv::Scalar(200, 0, 0), 2, 8, vector<Vec4i>(), 0, Point());

	rectangle(m->original, hg->bRect.tl(), hg->bRect.br(), Scalar(0, 0, 200));
	vector<Vec4i>::iterator d = hg->defects[hg->cIdx].begin();


	vector<Mat> channels;
	Mat result;
	for (int i = 0; i < 3; i++)
		channels.push_back(m->binary);
	merge(channels, result);
	drawContours(result, hg->hullPoint, hg->cIdx, cv::Scalar(0, 0, 250), 10, 8, vector<Vec4i>(), 0, Point());


	while (d != hg->defects[hg->cIdx].end()) {
		Vec4i& v = (*d);
		int startidx = v[0]; Point ptStart(hg->contours[hg->cIdx][startidx]);
		int endidx = v[1]; Point ptEnd(hg->contours[hg->cIdx][endidx]);
		int faridx = v[2]; Point ptFar(hg->contours[hg->cIdx][faridx]);
		float depth = v[3] / 256;
		circle(result, ptFar, 9, Scalar(0, 205, 0), 5);

		d++;
	}
}

void makeContours(ImageSource *m, HandGesture* hg) {
	Mat aBw;
	pyrUp(m->binary, m->binary);
	m->binary.copyTo(aBw);
	findContours(aBw, hg->contours, CV_RETR_EXTERNAL, CV_CHAIN_APPROX_NONE);
	hg->initVectors();
	hg->cIdx = findBiggestContour(hg->contours);
	if (hg->cIdx != -1) {
		hg->bRect = boundingRect(Mat(hg->contours[hg->cIdx]));
		convexHull(Mat(hg->contours[hg->cIdx]), hg->hullPoint[hg->cIdx], false, true);
		convexHull(Mat(hg->contours[hg->cIdx]), hg->hullIndex[hg->cIdx], false, false);
		approxPolyDP(Mat(hg->hullPoint[hg->cIdx]), hg->hullPoint[hg->cIdx], 18, true);
		if (hg->contours[hg->cIdx].size() > 3) {
			convexityDefects(hg->contours[hg->cIdx], hg->hullIndex[hg->cIdx], hg->defects[hg->cIdx]);
			hg->eleminateDefects();
		}
		bool isHand = hg->detectIfHand();
		hg->printGestureInfo(m->original);
		if (isHand) {
			hg->getFingertips(m);
			hg->drawFingertips(m);
			myDrawContours(m, hg);
		}
	}
}

extern "C" int Init(int& outCameraWidth, int& outCameraHeight)
{
	_imageSource = ImageSource(0);
	_currentState = NONE;

	if (!_imageSource.capture.isOpened())
	{
		return 1;
	}

	_squareLength = 20;

	outCameraWidth = _imageSource.capture.get(CAP_PROP_FRAME_WIDTH);
	outCameraHeight = _imageSource.capture.get(CAP_PROP_FRAME_HEIGHT);

	return 0;
}

extern "C" void SetupForGettingColorSample()
{
	_imageSource.capture >> _imageSource.original;

	ImageSource* imageSrc = &_imageSource;
	colorSampleRegions.push_back(ColorSampleROI(Point(imageSrc->original.cols / 3, imageSrc->original.rows / 6), Point(imageSrc->original.cols / 3 + _squareLength, imageSrc->original.rows / 6 + _squareLength), imageSrc->original));
	colorSampleRegions.push_back(ColorSampleROI(Point(imageSrc->original.cols / 4, imageSrc->original.rows / 2), Point(imageSrc->original.cols / 4 + _squareLength, imageSrc->original.rows / 2 + _squareLength), imageSrc->original));
	colorSampleRegions.push_back(ColorSampleROI(Point(imageSrc->original.cols / 3, imageSrc->original.rows / 1.5), Point(imageSrc->original.cols / 3 + _squareLength, imageSrc->original.rows / 1.5 + _squareLength), imageSrc->original));
	colorSampleRegions.push_back(ColorSampleROI(Point(imageSrc->original.cols / 2, imageSrc->original.rows / 2), Point(imageSrc->original.cols / 2 + _squareLength, imageSrc->original.rows / 2 + _squareLength), imageSrc->original));
	colorSampleRegions.push_back(ColorSampleROI(Point(imageSrc->original.cols / 2.5, imageSrc->original.rows / 2.5), Point(imageSrc->original.cols / 2.5 + _squareLength, imageSrc->original.rows / 2.5 + _squareLength), imageSrc->original));
	colorSampleRegions.push_back(ColorSampleROI(Point(imageSrc->original.cols / 2, imageSrc->original.rows / 1.5), Point(imageSrc->original.cols / 2 + _squareLength, imageSrc->original.rows / 1.5 + _squareLength), imageSrc->original));
	colorSampleRegions.push_back(ColorSampleROI(Point(imageSrc->original.cols / 2.5, imageSrc->original.rows / 1.8), Point(imageSrc->original.cols / 2.5 + _squareLength, imageSrc->original.rows / 1.8 + _squareLength), imageSrc->original));
	_currentState = AWAITNG_PALM;
}

extern "C" void StartColorSampling() {
    _currentState = GETTING_COLOR_SAMPLE;
}

extern "C" void StartTracking() {
    for (int i = 0; i < NSAMPLES; i++) {
        c_lower[i][0] = 7;
        c_upper[i][0] = 21;
        c_lower[i][1] = 0;
        c_upper[i][1] = 16;
        c_lower[i][2] = 5;
        c_upper[i][2] = 10;
    }
    _currentState = TRACKING;
}

void GetColorSample(ImageSource *imageSrc) {
    cvtColor(imageSrc->original, imageSrc->original, ORIGCOL2COL);
    for (int j = 0; j < NSAMPLES; j++) {
        getAvgColor(colorSampleRegions[j], avgColor[j]);
        colorSampleRegions[j].draw_rectangle(imageSrc->original);
    }
    cvtColor(imageSrc->original, imageSrc->original, COL2ORIGCOL);
    string imgText = string("Finding average color of hand");
    putText(imageSrc->original, imgText, Point(imageSrc->original.cols / 2, imageSrc->original.rows / 10), _fontFace, 1.2f, Scalar(200, 0, 0), 2);
    _imageSource.displayed = _imageSource.original.clone();
}

void Tracking(ImageSource imageSrc) {
    pyrDown(imageSrc.original, imageSrc.downsampled);
    blur(imageSrc.downsampled, imageSrc.downsampled, Size(3, 3));
    cvtColor(imageSrc.downsampled, imageSrc.downsampled, ORIGCOL2COL);
    produceBinaries(&imageSrc);
    cvtColor(imageSrc.downsampled, imageSrc.downsampled, COL2ORIGCOL);
    makeContours(&imageSrc, &_handGesture);
    _handGesture.getFingerNumber(&imageSrc);
    pyrDown(imageSrc.binary, imageSrc.binary);
    pyrDown(imageSrc.binary, imageSrc.binary);
    Rect roi(Point(3 * imageSrc.original.cols / 4, 0), imageSrc.binary.size());
    vector<Mat> channels;
    Mat result;
    for (int i = 0; i < 3; i++)
        channels.push_back(imageSrc.binary);
    merge(channels, result);
    result.copyTo(imageSrc.original(roi));
    _imageSource.displayed = _imageSource.original.clone();
}

void DrawColorSampleRegions(ImageSource *imageSrc) {
	for (int i = 0; i < NSAMPLES; i++) {
		colorSampleRegions[i].draw_rectangle(_imageSource.original);
	}
	string imgText = string("Cover rectangles with palm");
	putText(imageSrc->original, imgText, Point(imageSrc->original.cols / 2, imageSrc->original.rows / 10), _fontFace, 1.2f, Scalar(200, 0, 0), 2);
    _imageSource.displayed = _imageSource.original.clone();
}

extern "C" void  GetFrame(PixelData* pixels)
{
	_imageSource.capture >> _imageSource.original;

	if (_imageSource.original.empty() || _imageSource.original.depth() != CV_8U)
		return;
    
    ImageSource* imageSrc = &_imageSource;
    
    flip(imageSrc->original, imageSrc->original, 1);

    switch (_currentState) {
    case AWAITNG_PALM:
        DrawColorSampleRegions(imageSrc);
        break;
    case GETTING_COLOR_SAMPLE:
        GetColorSample(imageSrc);
        break;
    case TRACKING:
        Tracking(_imageSource);
        break;
    default:
        _imageSource.displayed = _imageSource.original.clone();
        break;
    }

    int counter = 0;
    MatIterator_<Vec3b> it, end;
    for( it = _imageSource.displayed.begin<Vec3b>(), end = _imageSource.displayed.end<Vec3b>(); it != end; ++it)
    {
        *(pixels + counter) = PixelData ((*it)[2], (*it)[1], (*it)[0]);
        counter++;
    }
}

extern "C" void  Close()
{
	_imageSource.capture.release();
}
