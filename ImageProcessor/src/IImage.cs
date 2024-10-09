using System.Drawing;
using OpenCvSharp;

namespace ImageProcessor
{
    public interface IImage
    {
        /// <summary>
        /// 获取图像的宽度
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 获取图像的高度
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 将图像转换为 Bitmap 格式
        /// </summary>
        /// <returns></returns>
        Bitmap ToBitmap();

        // 将图像转换为 OpenCV Mat 格式
        Mat ToMat();

        // 从 Bitmap 加载图像
        void LoadFromBitmap(Bitmap bitmap);

        // 从 OpenCV Mat 加载图像
        void LoadFromMat(Mat mat);

        // 获取图像的原始数据
        byte[] GetRawData();

        // 设置图像的原始数据
        void SetRawData(byte[] data, int width, int height);
    }
}