using System;
using OpenCvSharp;
using Point = System.Drawing.Point;

namespace ImageProcessor
{
    public class ImageProcessor : IImageProcessor
    {
        private Image _image; // 因为会用到派生类的方法，所以在这里不能是接口

        /// <summary>
        /// 加载图像以创建处理器实例，以后续链式调用
        /// </summary>
        /// <param name="image"></param>
        public ImageProcessor(Image image)
        {
            _image = image; // OOP多态
        }


        public IImageProcessor Translate(double offsetX, double offsetY)
        {
            if (offsetX > _image.Width || offsetY > _image.Height)
            {
                throw new ArgumentException("偏移尺寸超过图像最大座标！");
            }
            
            throw new NotImplementedException();
        }

        public IImageProcessor Rotate(double angle)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor Overlay(IImage baseImage, IImage overlayImage, int x, int y)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor SmoothEdges(int radius)
        {
            throw new NotImplementedException();
        }

        public IImageProcessor Scale(double scaleX, double scaleY)
        {
            if (_image == null)
            {
                throw new InvalidOperationException("图像为空，无法缩放。");
            }

            // 计算缩放后的尺寸
            var newSize = new Size(
                (int)(scaleX * _image.Width),
                (int)(scaleY * _image.Height));

            // 创建输出图像
            var resizedImage = new Mat();

            // 使用 OpenCvSharp 的 Resize 方法进行缩放
            Cv2.Resize(_image.ToMat(), resizedImage, newSize);

            // 返回新的图像处理器实例
            return new ImageProcessor(resizedImage);
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

        public IImageProcessor ApplyFilter(Func<IImage, IImage> filter)
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

        public IImage Process()
        {
            throw new NotImplementedException();
        }
    }
}