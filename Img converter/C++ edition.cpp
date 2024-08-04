#include <opencv2/opencv.hpp>
#include <iostream>
#include <string>
#include <vector>
#include <filesystem>
#pragma warning (disable:4996)

using namespace cv;
using namespace std;
using namespace filesystem;

int main() 
{
    string directory = current_path().string();
    vector<std::string> files;

    for (const auto& entry : directory_iterator(directory))
        if (entry.path().extension() == ".jpg" || entry.path().extension() == ".png")
            files.push_back(entry.path().string());

    printf("Choose Instruction:\n 0 - Grey\n 1 - Transparent\n 2 - Stroke\n");
    int cnt; scanf("%d", &cnt);

    switch (cnt)
    {
        case 0:
            puts("Input if reverse");
            int reverse; scanf("%d", &reverse);
            for (const auto& file : files)
            {
                Mat src = imread(file, IMREAD_UNCHANGED);
                for (int i = 0; i < src.rows; i++)
                    for (int j = 0; j < src.cols; j++)
                    {
                        Vec3b& color = src.at<Vec3b>(i, j);
                        int grayValue = (color[0] + color[1] + color[2]) / 3;
                        if (reverse) grayValue = 255 - grayValue;
                        color[0] = color[1] = color[2] = grayValue;
                    }
                imwrite(file.substr(0, file.find_last_of('.')) + "_new.png", src);
            }
            break;
        case 1:
            puts("Input transparent value : ");
            int transparentValue; scanf("%d", &transparentValue);

            for (const auto& file : files)
            {
                Mat src = imread(file, IMREAD_UNCHANGED), dst(src.size(), CV_8UC4);
                for (int i = 0; i < src.rows; i++)
                    for (int j = 0; j < src.cols; j++)
                    {
                        Vec4b pixel = src.channels() == 4 ? src.at<Vec4b>(i, j) : Vec4b(src.at<Vec3b>(i, j)[0], src.at<Vec3b>(i, j)[1], src.at<Vec3b>(i, j)[2], 255);
                        int sumValue = pixel[0] + pixel[1] + pixel[2];
                        if (sumValue > transparentValue) dst.at<Vec4b>(i, j) = cv::Vec4b(255, 255, 255, 0);
                        else dst.at<Vec4b>(i, j) = pixel;
                    }
                imwrite(file.substr(0, file.find_last_of('.')) + "_new.png", dst);
            }
            break;
        case 2:
            puts("Input stroke value : ");
            int strokeValue; scanf("%d", &strokeValue);
            for (const auto& file : files)
            {
                Mat src = imread(file, IMREAD_UNCHANGED), dst(src.size(), CV_8UC4), tmp = Mat::zeros(src.size(), CV_64F);
                for (int t = 0; t < strokeValue; t++)
                {
                    for (int i = 0; i < src.rows; i++)
                        for (int j = 0; j < src.cols; j++)
                        {
                            int sumValue = src.at<Vec3b>(i, j)[0] + src.at<Vec3b>(i, j)[1] + src.at<Vec3b>(i, j)[2];
                            tmp.at<double>(i, j) = 765.0 - sumValue
                                + (i > 0 ? tmp.at<double>(i - 1, j) : 0)
                                + (j > 0 ? tmp.at<double>(i, j - 1) : 0)
                                - (i > 0 && j > 0 ? tmp.at<double>(i - 1, j - 1) : 0);
                        }

                    for (int i = 0; i < src.rows; i++)
                        for (int j = 0; j < src.cols; j++)
                        {
                            int li = max(i - strokeValue, 0), ri = min(i + strokeValue, src.rows - 1),
                                lj = max(j - strokeValue, 0), rj = min(j + strokeValue, src.cols - 1);
                            double areaSum = tmp.at<double>(ri, rj)
                                - (li > 0 ? tmp.at<double>(li - 1, rj) : 0)
                                - (lj > 0 ? tmp.at<double>(ri, lj - 1) : 0)
                                + (li > 0 && lj > 0 ? tmp.at<double>(li - 1, lj - 1) : 0);
                            int numPixels = (ri - li + 1) * (rj - lj + 1); double average = areaSum / numPixels;
                            Vec3b& color = src.at<Vec3b>(i, j); int r = max(color[2] - static_cast<int>(average), 0),
                                g = max(color[1] - static_cast<int>(average), 0), b = max(color[0] - static_cast<int>(average), 0);
                            dst.at<Vec4b>(i, j) = Vec4b(b, g, r, src.channels() == 4 ? src.at<Vec4b>(i, j)[3] : 255);
                        }
                    printf("Loop %d complete\n", t + 1);
                }

                for (int i = 0; i < dst.rows; i++)
                    for (int j = 0; j < dst.cols; j++)
                    {
                        Vec4b& pixel = dst.at<cv::Vec4b>(i, j);
                        if (pixel[0] + pixel[1] + pixel[2] < 100) pixel[3] = 255;
                    }

                imwrite(file.substr(0, file.find_last_of('.')) + "_new.png", dst);
            }
            break;
    }
    puts("Instruction finished");
}