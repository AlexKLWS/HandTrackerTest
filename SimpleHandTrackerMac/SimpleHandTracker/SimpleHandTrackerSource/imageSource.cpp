#include "imageSource.hpp"
#include <opencv2/imgproc/imgproc.hpp>
#include<opencv2/opencv.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <stdio.h>
#include <stdlib.h>
#include <iostream>
#include <string>

using namespace cv;

ImageSource::ImageSource() {
}

ImageSource::ImageSource(int webCamera) {
	cameraSrc = webCamera;
	capture.open(webCamera);
}

