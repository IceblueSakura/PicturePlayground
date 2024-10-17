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
        /// <param name="offsetX">平移的X轴像素数</param>
        /// <param name="offsetY">平移的X轴像素数</param>
        /// <returns></returns>
        IImageProcessor Translate(double offsetX, double offsetY);

        /// <summary>
        /// 按比例缩放
        /// </summary>
        /// <param name="scaleX">X轴缩放比例</param>
        /// <param name="scaleY">Y轴缩放比例</param>
        /// <returns>缩放后的图像处理器实例</returns>
        IImageProcessor Scale(double scaleX, double scaleY);

        /// <summary>
        /// 设置旋转中心座标，如果不设置则为图像几何中心
        /// </summary>
        /// <param name="centerX">X轴中心座标</param>
        /// <param name="centerY">Y轴中心座标</param>
        /// <returns></returns>
        IImageProcessor SetRotationCenter(int centerX, int centerY);

        /// <summary>
        /// 将图像旋转特定角度，空白处使用0填充
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns></returns>
        IImageProcessor Rotate(double angle);

        /// <summary>
        /// 将overlayImage覆盖到当前实例中图像的指定座标，会对overlayImage取候选边距后再合并以避免操作后白色底色
        /// 这是v1版本，使用简单矩阵算法，仅扣白色边缘，运行速度快
        /// </summary>
        /// <param name="overlayImage"></param>
        /// <param name="x">在当前实例中图像(背景)的X轴座标</param>
        /// <param name="y">在当前实例中图像(背景)的Y轴座标</param>
        /// <returns></returns>
        IImageProcessor Overlay(Mat overlayImage, int x, int y);

        /// <summary>
        /// 将overlayImage覆盖到当前实例中图像的指定座标，会对overlayImage取候选边距后再合并以避免操作后白色底色
        /// 这是v2版本，使用Canny边缘检测分割边缘，更准确，但速度慢，v1不好使再用这个
        /// </summary>
        /// <param name="overlayImage"></param>
        /// <param name="x">在当前实例中图像(背景)的X轴座标</param>
        /// <param name="y">在当前实例中图像(背景)的Y轴座标</param>
        /// <returns></returns>
        IImageProcessor OverlayV2(Mat overlayImage, int x, int y);
        
        /// <summary>
        /// 裁切一块矩形区域
        /// </summary>
        /// <param name="startX">区域左上X座标</param>
        /// <param name="startY">区域左上Y座标</param>
        /// <param name="width">裁剪区域宽度</param>
        /// <param name="height">裁剪区域高度</param>
        /// <returns></returns>
        IImageProcessor CropRectangle(int startX, int startY, int width, int height);
        
        /// <summary>
        /// 获取当前实例中保存的Bitmap形式图像
        /// </summary>
        /// <returns>处理后的图像</returns>
        Bitmap GetBitmap();

        /// <summary>
        /// 获取当前实例中保存的Mat形式图像，获取时不会进行复制
        /// </summary>
        /// <returns></returns>
        Mat GetMat();

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