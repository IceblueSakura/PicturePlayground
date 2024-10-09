using System.Drawing;
using OpenCvSharp;

namespace ImageProcessor
{
    public class Image : IImage
    {
        public int Width { get; }
        public int Height { get; }
        public Bitmap ToBitmap()
        {
            throw new System.NotImplementedException();
        }

        public Mat ToMat()
        {
            throw new System.NotImplementedException();
        }

        public void LoadFromBitmap(Bitmap bitmap)
        {
            throw new System.NotImplementedException();
        }

        public void LoadFromMat(Mat mat)
        {
            throw new System.NotImplementedException();
        }

        public byte[] GetRawData()
        {
            throw new System.NotImplementedException();
        }

        public void SetRawData(byte[] data, int width, int height)
        {
            throw new System.NotImplementedException();
        }
    }
}