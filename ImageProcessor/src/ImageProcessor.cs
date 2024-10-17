using System;
using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using Point = OpenCvSharp.Point;
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

            // 应用仿射变换，变换时线性插值，填充常量为0(白色)
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
            Cv2.WarpAffine(_image, rotatedImage, rotationMatrix, _image.Size(),
                InterpolationFlags.Linear, // 线性插值
                BorderTypes.Constant, // 常量填充，还可以选复制填充(填充为临近值)和反射填充(填充为边界反射值)
                new Scalar(0) // 填充的常量值，方便后续精确ROI进行覆盖
            );
            _image = rotatedImage; // 覆盖原本的图像  

            return this;
        }

        public IImageProcessor Overlay(Mat overlayImage, int x, int y)
        {
            // 计算起始位置，避免座标错误
            var startX = Math.Max(x, 0);
            var startY = Math.Max(y, 0);

            // 边界检查，如果覆盖大于背景，直接返回不做处理
            if (startX >= _image.Width || startY >= _image.Height)
            {
                return this;
            }

            // 对overlay计算有效区域，避免覆盖后的边缘空白(空白可能由几何变幻造成)
            var mask = new Mat(); // 二值掩摸矩阵
            Cv2.Threshold(overlayImage, mask,
                1, // 判断阈值，只要不是白色
                255, // 填充颜色，作为候选区域标记
                ThresholdTypes.Binary // 二值化分割
            );

            // 查找轮廓
            Point[][] contours; // 轮廓的点集
            HierarchyIndex[] hierarchy;
            Cv2.FindContours(mask, out contours, // 找到的轮廓
                out hierarchy, // 轮廓层次信息
                RetrievalModes.External, // 只检索最外侧轮廓
                ContourApproximationModes.ApproxSimple // 简单轮廓近似模式
            );

            // 创建一个与background相同大小的掩码
            var overlayMask = new Mat(_image.Size(), MatType.CV_8UC1, Scalar.All(0));

            // 将找到的overlay轮廓绘制到掩码上，之后用于覆盖到background区域定位
            Cv2.DrawContours(overlayMask, contours, 0, Scalar.White, -1,
                LineTypes.AntiAlias, // 抗锯齿线条
                null,
                1 // 轮廓级别，1=只画最外侧线条
            );

            // 将overlay图像复制到background上，只覆盖抠出来的轮廓内的像素
            overlayImage.CopyTo(_image, overlayMask);
            return this;
        }

        public IImageProcessor OverlayV2(Mat overlayImage, int x, int y)
        {
            // 计算起始位置，避免座标错误
            var startX = Math.Max(x, 0);
            var startY = Math.Max(y, 0);

            // 边界检查，如果覆盖大于背景，直接返回不做处理
            if (startX >= _image.Width || startY >= _image.Height)
            {
                return this;
            }

            // 转换HSV，方便之后分割边缘。H=色调(0-180), S=饱和度(0-255), V=明度(0-255)
            var hsvImage = new Mat();
            Cv2.CvtColor(overlayImage, hsvImage, ColorConversionCodes.BGR2HSV);

            // 通过HSV的分布估计背景颜色，适用于背景可能不是白色的情况，HSV更适合颜色分割
            // 计算图像的直方图
            var hist = new Mat();
            int[] channels = { 0, 1 }; // 使用 HS 通道
            int[] histSize = { 32, 32 }; // 直方图的 bin 数量
            Rangef[] ranges = { new Rangef(0, 180), new Rangef(0, 256) }; // 定义为HSV的数值范围
            Cv2.CalcHist(new[] { hsvImage }, channels, null, hist, 2, histSize, ranges);  // 计算直方图，用于颜色统计

            // 找到直方图中最大值对应的位置
            Cv2.MinMaxLoc(hist, out _, out var maxVal, out _, out var maxLoc);

            // 根据颜色分布情况，计算背景颜色的 HS 值
            var backgroundH = (int)(maxLoc.X * (180.0 / histSize[0]));
            var backgroundS = (int)(maxLoc.Y * (256.0 / histSize[1]));

            // 定义背景颜色的 HS 范围，用于分割颜色的精度调整
            const int hRange = 10; // 低色调(0-180，每60一个颜色，对应红绿蓝三色）
            const int sRange = 30; // 低饱和度，灰一些的区域

            // 定义背景颜色的HS范围
            var lowerBackground = new Scalar(backgroundH - hRange, backgroundS - sRange, 0);
            var upperBackground = new Scalar(backgroundH + hRange, backgroundS + sRange, 255);

            // 创建掩码
            var mask = new Mat();
            Cv2.InRange(hsvImage, lowerBackground, upperBackground, mask);  // 根据颜色范围生成mask，背景=255(白)，前景=0(黑)

            // 反转掩码，提取物体
            var objectMask = new Mat();  // 颜色分割的区域
            Cv2.BitwiseNot(mask, objectMask);

            // 使用 Canny 边缘检测生成掩码
            var edges = new Mat();  // 边缘检测分割的区域
            Cv2.Canny(overlayImage, edges, // 根据梯度分割边缘
                50, // 弱边缘阈值，当与强边缘连接时才保留
                250 // 强边缘阈值，最好是弱边缘的2倍以上，这是肯定的边缘梯度阈值
            );

            // 使用膨胀操作扩展边缘
            var dilatedEdges = new Mat();
            var kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3)); // 膨胀区域3x3 pixel
            Cv2.Dilate(edges, dilatedEdges, kernel, iterations: 2); // 膨胀操作，扩展边缘，扩展迭代2次

            // 使用腐蚀操作细化边缘
            var erodedEdges = new Mat();
            Cv2.Erode(dilatedEdges, erodedEdges, kernel, iterations: 1); // 腐蚀操作，细化边缘， 迭代1次

            // 将颜色分割掩码和边缘检测结果结合起来
            var finalMask = new Mat();
            Cv2.BitwiseAnd(objectMask, erodedEdges, finalMask);

            // 根据最终合并结果查找物体轮廓
            Cv2.FindContours(mask, out var contours, // 找到的轮廓
                out _, // 轮廓层次信息
                RetrievalModes.External, // 只检索最外侧轮廓
                ContourApproximationModes.ApproxSimple // 简单轮廓近似模式
            );

            // 创建一个与background相同大小的掩码
            var overlayMask = new Mat(_image.Size(), MatType.CV_8UC1, Scalar.All(0));

            // 将找到的overlay轮廓绘制到掩码上，之后用于覆盖到background区域定位
            Cv2.DrawContours(overlayMask, contours, 0, Scalar.White, -1,
                LineTypes.AntiAlias, // 抗锯齿线条
                null,
                1 // 轮廓级别，1=只画最外侧线条
            );

            // 将overlay图像复制到background上，只覆盖抠出来的轮廓内的像素
            overlayImage.CopyTo(_image, overlayMask);
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

        public Mat GetMat()
        {
            return _image;
        }
    }
}