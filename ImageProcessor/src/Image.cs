using System.Drawing;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace ImageProcessor
{
    public class Image : IImage
    {
        private Mat _mat;

        public int Width => _mat?.Cols ?? 0;
        public int Height => _mat?.Rows ?? 0;

        /// <summary>
        /// 构造方法
        /// </summary>
        public Image()
        {
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="bitmap"></param>
        public Image(Bitmap bitmap)
        {
            LoadFromBitmap(bitmap);
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="mat"></param>
        public Image(Mat mat)
        {
            LoadFromMat(mat);
        }

        /// <summary>
        /// OpenCV到Bitmap的转换实现
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        private Bitmap ConvertMatToBitmap(Mat mat)
        {
            return _mat.ToBitmap();
        }

        /// <summary>
        ///  Bitmap到OpenCV的Mat转换实现
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        private Mat ConvertBitmapToMat(Bitmap bitmap)
        {
            return bitmap.ToMat();
        }

        public Bitmap ToBitmap()
        {
            return ConvertMatToBitmap(_mat); // 调用方法实现的私有转换方法
        }

        public Mat ToMat()
        {
            return _mat;
        }

        public void LoadFromPath(string path)
        {
            _mat = Cv2.ImRead(path);
        }

        public void LoadFromBitmap(Bitmap bitmap)
        {
            _mat = ConvertBitmapToMat(bitmap);
        }

        public void LoadFromMat(Mat mat)
        {
            _mat = mat;
        }

        public byte[] GetRawData(string format = ".bmp")
        {
            return ToMat().ToBytes(format);
        }

        public void SetRawData(byte[] data, ImreadModes colorMode = ImreadModes.Color)
        {
            _mat = Mat.FromImageData(data, colorMode);
        }

        /// <summary>
        /// 隐式类型转换，Image->Mat
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public static implicit operator Mat(Image image)
        {
            return image.ToMat();
        }

        /// <summary>
        /// 隐式类型转换，Mat->Image
        /// </summary>
        /// <param name="mat"></param>
        /// <returns></returns>
        public static implicit operator Image(Mat mat)
        {
            return new Image(mat);
        }
    }
}