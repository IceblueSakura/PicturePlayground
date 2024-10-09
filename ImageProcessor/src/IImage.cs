using System.Drawing;
using OpenCvSharp;

namespace ImageProcessor
{
    public interface IImage
    {
        /// <summary>
        /// 图像的宽度
        /// </summary>
        int Width { get; }

        /// <summary>
        /// 图像的高度
        /// </summary>
        int Height { get; }

        /// <summary>
        /// 将图像转换为 Bitmap 格式
        /// </summary>
        /// <returns><c>System.Drawing::Bitmap</c>instance</returns>
        Bitmap ToBitmap();

        /// <summary>
        /// 将图像转换为 OpenCV Mat 格式
        /// </summary>
        /// <returns><c>OpenCvSharp::Mat</c></returns>
        Mat ToMat();

        /// <summary>
        /// 从指定路径加载图像
        /// </summary>
        /// <param name="path"></param>
        void LoadFromPath(string path);

        /// <summary>
        /// 从 Bitmap 加载图像
        /// </summary>
        /// <param name="bitmap"><c>System.Drawing::Bitmap</c></param>
        void LoadFromBitmap(Bitmap bitmap);

        /// <summary>
        /// 从 OpenCV Mat 加载图像
        /// </summary>
        /// <param name="mat"><c>OpenCvSharp::Mat</c></param>
        void LoadFromMat(Mat mat);

        /// <summary>
        /// 获取图像的原始数据
        /// </summary>
        /// <param name="format">格式，如.png</param>
        /// <returns><c>byte[]</c></returns>
        byte[] GetRawData(string format=".bmp");

        /// <summary>
        /// 设置图像的原始数据
        /// </summary>
        /// <param name="data"><c>byte[]</c> data</param>
        /// <param name="colorMode">色彩模式，如灰度</param>
        void SetRawData(byte[] data, ImreadModes colorMode = ImreadModes.Color);
    }
}