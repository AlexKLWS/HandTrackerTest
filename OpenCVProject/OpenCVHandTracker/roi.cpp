#include <opencv2/imgproc/imgproc.hpp>
#include<opencv2/opencv.hpp>
#include <opencv2/highgui/highgui.hpp>
#include <stdio.h>
#include <stdlib.h>
#include <iostream>
#include <string>
#include "roi.hpp"

using namespace cv;
using namespace std;

ColorSampleROI::ColorSampleROI() {
	upper_corner = Point(0, 0);
	lower_corner = Point(0, 0);

}

ColorSampleROI::ColorSampleROI(Point u_corner, Point l_corner, Mat src) {
	upper_corner = u_corner;
	lower_corner = l_corner;
	color = Scalar(0, 255, 0);
	border_thickness = 2;
	roi_ptr = src(Rect(u_corner.x, u_corner.y, l_corner.x - u_corner.x, l_corner.y - u_corner.y));
}

void ColorSampleROI::draw_rectangle(Mat src) {
	rectangle(src, upper_corner, lower_corner, color, border_thickness);

}
