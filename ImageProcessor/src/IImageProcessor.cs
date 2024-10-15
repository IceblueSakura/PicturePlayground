using System;
using System.Drawing;
using OpenCvSharp;

namespace ImageProcessor
{
    public interface IImageProcessor
    {
        /// <summary>
        /// 将图像平移,空白处使用0填充
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <returns></returns>
        IImageProcessor Translate(double offsetX, double offsetY);

        /// <summary>
        /// 缩放
        /// </summary>
        /// <param name="scaleX">X轴缩放比例</param>
        /// <param name="scaleY">Y轴缩放比例</param>
        /// <returns>缩放后的图像处理器实例</returns>
        IImageProcessor Scale(double scaleX, double scaleY);

        /// <summary>
        /// 设置旋转中心座标
        /// </summary>
        /// <param name="centerX">X轴中心座标</param>
        /// <param name="centerY">Y轴中心座标</param>
        /// <returns></returns>
        IImageProcessor SetRotationCenter(int centerX, int centerY);

        /// <summary>
        /// 将图像旋转特定角度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        IImageProcessor Rotate(double angle);

        /// <summary>
        /// 覆盖图像
        /// </summary>
        /// <param name="overlayImage"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        IImageProcessor Overlay(Mat overlayImage, int x, int y);

        /// <summary>
        /// 裁切区域
        /// </summary>
        /// <param name="startX"></param>
        /// <param name="startY"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        IImageProcessor CropRectangle(int startX, int startY, int width, int height);
        
        /// <summary>
        /// 获取Bitmap
        /// </summary>
        /// <returns>处理后的图像</returns>
        Bitmap GetBitmap();
        
        // /// <summary>
        // /// 平滑图像边缘
        // /// </summary>
        // /// <param name="radius">平滑像素半径</param>
        // /// <returns></returns>
        // IImageProcessor SmoothEdges(int radius);

        // /// <summary>
        // /// 调整透明度
        // /// </summary>
        // /// <param name="opacity"></param>
        // /// <returns></returns>
        // IImageProcessor SetOpacity(double opacity);

        // /// <summary>
        // /// 水平或垂直翻转图像
        // /// </summary>
        // /// <param name="horizontal">水平翻转</param>
        // /// <param name="vertical">垂直翻转</param>
        // /// <returns></returns>
        // IImageProcessor Flip(bool horizontal, bool vertical);

        // /// <summary>
        // /// 裁切区域
        // /// </summary>
        // /// <param name="startX"></param>
        // /// <param name="startY"></param>
        // /// <param name="width"></param>
        // /// <param name="height"></param>
        // /// <returns></returns>
        // IImageProcessor Crop(int startX, int startY, int width, int height);

        // /// <summary>
        // /// 使用自定义滤镜
        // /// </summary>
        // /// <param name="filter"></param>
        // /// <returns></returns>
        // IImageProcessor ApplyFilter(Func<Mat, Mat> filter);

        // /// <summary>
        // /// 调整亮度和对比度
        // /// </summary>
        // /// <param name="brightness">亮度</param>
        // /// <param name="contrast">对比度</param>
        // /// <returns></returns>
        // IImageProcessor AdjustBrightnessContrast(double brightness, double contrast);


        // /// <summary>
        // /// 应用透视变换，将图像的四个角移动到新的位置，以实现透视效果。
        // /// </summary>
        // /// <param name="sourcePoints"></param>
        // /// <param name="destinationPoints"></param>
        // /// <returns></returns>
        // IImageProcessor PerspectiveTransform(Point[] sourcePoints, Point[] destinationPoints);

        // /// <summary>
        // /// 调整RGB颜色值，例如增强某个颜色强度。
        // /// </summary>
        // /// <param name="red"></param>
        // /// <param name="green"></param>
        // /// <param name="blue"></param>
        // /// <returns></returns>
        // IImageProcessor AdjustColor(int red, int green, int blue);


    }
}