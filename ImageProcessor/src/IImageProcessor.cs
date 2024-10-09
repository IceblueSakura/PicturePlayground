using System;
using System.Drawing;

namespace ImageProcessor
{
    public interface IImageProcessor
    {
        IImageProcessor Translate(double offsetX, double offsetY);
        IImageProcessor Rotate(double angle);
        IImageProcessor Overlay(Image baseImage, Image overlayImage, int x, int y);
        IImageProcessor SmoothEdges(int radius);
        IImageProcessor Scale(double scaleX, double scaleY);
        IImageProcessor SetOpacity(double opacity);
        IImageProcessor Flip(bool horizontal, bool vertical);
        IImageProcessor Crop(int startX, int startY, int width, int height);
        IImageProcessor ApplyFilter(Func<Image, Image> filter);
        IImageProcessor AdjustBrightnessContrast(double brightness, double contrast);
        IImageProcessor SetRotationCenter(int centerX, int centerY);
        IImageProcessor PerspectiveTransform(Point[] sourcePoints, Point[] destinationPoints);
        IImageProcessor AdjustColor(int red, int green, int blue);
        Image GetResult();
    }
}