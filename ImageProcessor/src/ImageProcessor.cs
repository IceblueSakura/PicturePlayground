using System;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Size = OpenCvSharp.Size;

namespace ImageProcessor
{
    public class ImageProcessor : IImageProcessor
    {
        private Mat _image;
        private Point2f _rotationCenter;

        public ImageProcessor(Mat image)
        {
            _image = image;
            // ReSharper disable once PossibleLossOfFraction
            _rotationCenter = new Point2f(image.Width / 2, image.Height / 2);
        }

        public ImageProcessor(Bitmap image)
        {
            _image = image.ToMat();
            _rotationCenter = new Point2f(image.Width / 2, image.Height / 2);
        }

        public IImageProcessor Translate(double offsetX, double offsetY)
        {
            // 创建一个平移矩阵
            var translationMatrix = new Mat(2, 3, MatType.CV_64FC1);
            translationMatrix.Set<double>(0, 0, 1);
            translationMatrix.Set<double>(0, 1, 0);
            translationMatrix.Set<double>(0, 2, offsetX);
            translationMatrix.Set<double>(1, 0, 0);
            translationMatrix.Set<double>(1, 1, 1);
            translationMatrix.Set<double>(1, 2, offsetY);

            // 应用仿射变换
            var translatedImage = new Mat();
            Cv2.WarpAffine(_image, translatedImage, translationMatrix, _image.Size(), InterpolationFlags.Linear,
                BorderTypes.Constant, new Scalar(0));
            _image = translatedImage; // 覆盖原本的图像
            return this;
        }


        public IImageProcessor Scale(double scaleX, double scaleY)
        {
            // 计算缩放后的图像尺寸
            var newWidth = _image.Width * scaleX;
            var newHeight = _image.Height * scaleY;

            // 创建一个新的Mat对象来存储缩放后的图像
            var scaledImage = new Mat();

            // 使用Cv2.Resize进行缩放
            Cv2.Resize(_image, scaledImage, new Size(newWidth, newHeight),
                interpolation: InterpolationFlags.Linear);

            _image = scaledImage; // 覆盖原本的图像  
            return this;
        }

        public IImageProcessor SetRotationCenter(int centerX, int centerY)
        {
            _rotationCenter = new Point2f(centerX, centerY);
            return this;
        }

        public IImageProcessor Rotate(double angle)
        {
            // 创建旋转矩阵
            var rotationMatrix = Cv2.GetRotationMatrix2D(_rotationCenter, angle, 1.0);

            // 应用旋转
            var rotatedImage = new Mat();
            Cv2.WarpAffine(_image, rotatedImage, rotationMatrix, _image.Size(), InterpolationFlags.Linear,
                BorderTypes.Constant, new Scalar(0));
            _image = rotatedImage; // 覆盖原本的图像  

            return this;
        }

        public IImageProcessor Overlay(Mat overlayImage, int x, int y)
        {
            // 确保覆盖图像在目标图像的范围内
            var overlayWidth = overlayImage.Width;
            var overlayHeight = overlayImage.Height;

            var startX = Math.Max(x, 0);
            var startY = Math.Max(y, 0);
            var endX = Math.Min(x + overlayWidth, _image.Width);
            var endY = Math.Min(y + overlayHeight, _image.Height);

            // 计算覆盖区域
            var overlayRect = new Rect(startX, startY, endX - startX, endY - startY);
            var targetRect = new Rect(startX - x, startY - y, endX - startX, endY - startY);

            // 将覆盖图像复制到目标图像上
            overlayImage[targetRect].CopyTo(_image[overlayRect]);

            return this;
        }

        public IImageProcessor CropRectangle(int startX, int startY, int width, int height)
        {
            // 确保裁切区域在图像范围内
            var endX = Math.Min(startX + width, _image.Width);
            var endY = Math.Min(startY + height, _image.Height);

            // 裁切图像
            _image = new Mat(_image, new Rect(startX, startY, endX - startX, endY - startY));
            return this;
        }

        public Bitmap GetBitmap()
        {
            return _image.ToBitmap();
        }
    }
}