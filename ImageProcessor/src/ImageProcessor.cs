using System;
using System.Drawing;

namespace ImageProcessor
{
    public class ImageProcessor : IImageProcessor
    {
        public IImageProcessor Translate(double offsetX, double offsetY)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor Rotate(double angle)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor Overlay(Image baseImage, Image overlayImage, int x, int y)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor SmoothEdges(int radius)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor Scale(double scaleX, double scaleY)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor SetOpacity(double opacity)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor Flip(bool horizontal, bool vertical)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor Crop(int startX, int startY, int width, int height)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor ApplyFilter(Func<Image, Image> filter)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor AdjustBrightnessContrast(double brightness, double contrast)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor SetRotationCenter(int centerX, int centerY)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor PerspectiveTransform(Point[] sourcePoints, Point[] destinationPoints)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor AdjustColor(int red, int green, int blue)
        {
            throw new NotImplementedException();
        }

        public Image GetResult()
        {
            throw new NotImplementedException();
        }
    }
}