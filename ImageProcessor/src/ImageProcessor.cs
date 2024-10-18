using System;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;

namespace ImageProcessor
{
    public class ImageProcessor : IImageProcessor
    {
        private Mat _backgroundImage;
        private Mat _foregroundImage;
        private Point _currentForegroundLocation;
        private Mat _processedImage;

        public IImageProcessor LoadBackground(Mat image)
        {
            _backgroundImage = image;
            return this;
        }

        public IImageProcessor LoadBackground(Bitmap image)
        {
            _backgroundImage = image.ToMat();
            return this;
        }

        public IImageProcessor LoadForeground(Mat image)
        {
            _foregroundImage = image;
            return this;
        }

        public IImageProcessor LoadForeground(Bitmap image)
        {
            _foregroundImage = ConvertBgrToBgra(image.ToMat());
            return this;
        }

        public IImageProcessor Overlay()
        {
            if (_backgroundImage == null || _foregroundImage == null)
            {
                throw new InvalidOperationException("Background or foreground image is not loaded.");
            }

            if (_currentForegroundLocation.X < 0 || _currentForegroundLocation.Y < 0) return this;
            if (_processedImage != _backgroundImage) _processedImage.Dispose();
            _processedImage = _backgroundImage.Clone();
            if (_foregroundImage.Channels() != 3)
            {
                // 处理带透明度的图像的合并和降维
                // 创建掩码，用于处理透明度
                var mask = new Mat();
                Cv2.CvtColor(_foregroundImage, mask, ColorConversionCodes.BGR2GRAY);
                Cv2.Threshold(mask, mask, 0, 255, ThresholdTypes.Binary);
                // 使用掩码将前景图覆盖到背景图上
                _foregroundImage.CopyTo(_processedImage.SubMat(_currentForegroundLocation.Y,
                    _currentForegroundLocation.Y + _foregroundImage.Rows,
                    _currentForegroundLocation.X, _currentForegroundLocation.X + _foregroundImage.Cols), mask);
                mask.Dispose();
            }
            else
            {
                _foregroundImage.CopyTo(_processedImage.SubMat(_currentForegroundLocation.Y,
                    _currentForegroundLocation.Y + _foregroundImage.Rows,
                    _currentForegroundLocation.X, _currentForegroundLocation.X + _foregroundImage.Cols));
            }

            return this;
        }

        public IImageProcessor Translate(int offsetX, int offsetY)
        {
            if (_foregroundImage == null)
            {
                throw new InvalidOperationException("Foreground image is not loaded.");
            }

            _currentForegroundLocation.X += offsetX;
            _currentForegroundLocation.Y += offsetY;

            // Ensure the new location does not exceed the background boundaries
            if (_currentForegroundLocation.X < 0) _currentForegroundLocation.X = 0;
            if (_currentForegroundLocation.Y < 0) _currentForegroundLocation.Y = 0;
            if (_currentForegroundLocation.X + _foregroundImage.Cols > _backgroundImage.Cols)
                _currentForegroundLocation.X = _backgroundImage.Cols - _foregroundImage.Cols;
            if (_currentForegroundLocation.Y + _foregroundImage.Rows > _backgroundImage.Rows)
                _currentForegroundLocation.Y = _backgroundImage.Rows - _foregroundImage.Rows;

            return this;
        }

        public IImageProcessor Scale(double scaleX, double scaleY)
        {
            if (_foregroundImage == null)
            {
                throw new InvalidOperationException("Foreground image is not loaded.");
            }

            var newWidth = (int)(_foregroundImage.Cols * scaleX);
            var newHeight = (int)(_foregroundImage.Rows * scaleY);

            _foregroundImage = _foregroundImage.Resize(new OpenCvSharp.Size(newWidth, newHeight));
            return this;
        }

        public IImageProcessor CropRectangle(int startX, int startY, int width, int height)
        {
            if (_foregroundImage == null)
            {
                throw new InvalidOperationException("Foreground image is not loaded.");
            }

            _foregroundImage = _foregroundImage.SubMat(startY, startY + height, startX, startX + width);
            return this;
        }

        public IImageProcessor Rotate(double angle)
        {
            if (_foregroundImage == null)
            {
                throw new InvalidOperationException("Foreground image is not loaded.");
            }

            // 确保输入图像为 BGRA，不是则转换
            var bgraImage = _foregroundImage.Channels() == 3 ? ConvertBgrToBgra(_foregroundImage) : _foregroundImage;


            // 计算旋转中心点
            var center = new Point2f((float)(_foregroundImage.Cols / 2.0), (float)(_foregroundImage.Rows / 2.0));

            // 获取旋转矩阵
            var rotationMatrix = Cv2.GetRotationMatrix2D(center, angle, 1.0);

            // 计算旋转后的图像尺寸
            var radians = angle * Math.PI / 180.0; // 将角度转换为弧度
            var sin = Math.Abs(Math.Sin(radians)); // 计算正弦值的绝对值
            var cos = Math.Abs(Math.Cos(radians)); // 计算余弦值的绝对值

            // 适当放大尺寸以防止边缘裁切
            var newWidth = (int)(Math.Ceiling(_foregroundImage.Cols * cos + _foregroundImage.Rows * sin));
            var newHeight = (int)(Math.Ceiling(_foregroundImage.Cols * sin + _foregroundImage.Rows * cos));

            // 调整旋转矩阵以包含平移部分
            // 调整旋转矩阵的平移部分，以确保旋转后的图像居中
            rotationMatrix.Set(0, 2, rotationMatrix.Get<double>(0, 2) + (newWidth / 2.0 - center.X));
            rotationMatrix.Set(1, 2, rotationMatrix.Get<double>(1, 2) + (newHeight / 2.0 - center.Y));

            // 创建一个新的 Mat 用于存储旋转后的图像
            var rotatedImage = new Mat(new OpenCvSharp.Size(newWidth, newHeight), MatType.CV_8UC4, Scalar.All(0));

            // 执行旋转操作，使用完全透明的填充
            Cv2.WarpAffine(bgraImage, rotatedImage, rotationMatrix,
                new OpenCvSharp.Size(newWidth, newHeight),
                InterpolationFlags.Linear, BorderTypes.Constant,
                Scalar.All(0) // 填充全透明
            );
            rotationMatrix.Dispose();
            // 更新前景图
            _foregroundImage.Dispose();
            _foregroundImage = rotatedImage;
            return this;
        }


        public Mat GetMat()
        {
            if (_foregroundImage != null)
            {
                Overlay();
            }
            else
            {
                _processedImage = _backgroundImage;
            }

            return _processedImage;
        }

        public Bitmap GetBitmap()
        {
            return GetMat().ToBitmap();
        }

        private static Mat ConvertBgrToBgra(Mat bgrImage)
        {
            // 创建一个新的 Mat 用于存储 BGRA 图像
            var bgraImage = new Mat(bgrImage.Size(), MatType.CV_8UC4);

            // 将 BGR 图像转换为 BGRA 图像
            Cv2.CvtColor(bgrImage, bgraImage, ColorConversionCodes.BGR2BGRA);

            return bgraImage;
        }
    }
}