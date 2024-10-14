using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageProcessor
{
    public class ImageProcessorWrapper : IDisposable
    {
        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateImageProcessor(IntPtr bitmapData, int width, int height, byte channel,
            bool useGPU);

        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DestroyImageProcessor(IntPtr processor);

        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetProcessedImage(IntPtr processor, out IntPtr data, out int width, out int height,
            out int channels);

        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Scale(IntPtr processor, double scaleX, double scaleY);

        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CropRectangle(IntPtr processor, int startX, int startY, int width, int height);

        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int SetRotationCenter(IntPtr processor, float centerX, float centerY);

        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Rotate(IntPtr processor, double angle);

        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Translate(IntPtr processor, double offsetX, double offsetY);

        private IntPtr _processorHandle;

        public ImageProcessorWrapper(Bitmap bitmap, bool useGpu = true)
        {
            byte channels = GetChannelCount(bitmap.PixelFormat);
            if (channels == 0)
            {
                throw new NotImplementedException("不支持的像素类型！");
            }

            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                bitmap.PixelFormat);

            try
            {
                _processorHandle =
                    CreateImageProcessor(bitmapData.Scan0, bitmap.Width, bitmap.Height, channels, useGpu);
                if (_processorHandle == IntPtr.Zero)
                {
                    throw new Exception("创建图像处理器失败！");
                }
            }
            finally
            {
                bitmap.UnlockBits(bitmapData);
            }
        }

        public Bitmap GetProcessedImage()
        {
            if (_processorHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException("图像处理器可能已经被释放了。无法获取图像处理器。");
            }

            var result = GetProcessedImage(_processorHandle, out IntPtr dataPtr, out var width, out var height,
                out var channels);
            if (result != 0)
            {
                throw new Exception("获取处理后的图像失败！");
            }

            PixelFormat pixelFormat;
            switch (channels)
            {
                case 3:
                    pixelFormat = PixelFormat.Format24bppRgb;
                    break;
                case 4:
                    pixelFormat = PixelFormat.Format32bppArgb;
                    break;
                default:
                    throw new InvalidOperationException("暂不支持处理的通道数！");
            }

            // 创建用于接收数据的托管数组
            byte[] managedArray = new byte[width * height * channels];
            // 将非托管数据复制到托管数组
            Bitmap bitmap = new Bitmap(width, height, pixelFormat);
            var bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, width, height),
                ImageLockMode.WriteOnly,
                pixelFormat);
            try
            {
                // 将未托管的cpp数组复制到托管的数组中
                Marshal.Copy(dataPtr, managedArray, 0, managedArray.Length);
                // 将托管数组复制到Bitmap的内存中
                Marshal.Copy(managedArray, 0, bitmapData.Scan0, managedArray.Length);
            }
            finally
            {
                bitmap.UnlockBits(bitmapData); // 解锁Bitmap
                Marshal.FreeHGlobal(dataPtr);
            }

            return bitmap;
        }

        private byte GetChannelCount(PixelFormat pixelFormat)
        {
            switch (pixelFormat)
            {
                case PixelFormat.Format24bppRgb:
                    return 3;
                case PixelFormat.Format32bppArgb:
                    return 4;
                case PixelFormat.Format8bppIndexed:
                    return 1;
                default:
                    return 0;
            }
        }

        public ImageProcessorWrapper Scale(double scaleX, double scaleY)
        {
            if (_processorHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException("图像处理器可能已经被释放了。无法获取图像处理器。");
            }

            if (Scale(_processorHandle, scaleX, scaleY) != 0)
            {
                throw new Exception("Failed to scale image.");
            }

            return this;
        }

        public ImageProcessorWrapper CropRectangle(int startX, int startY, int width, int height)
        {
            if (_processorHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException("图像处理器可能已经被释放了。无法获取图像处理器。");
            }

            if (CropRectangle(_processorHandle, startX, startY, width, height) != 0)
            {
                throw new Exception("Failed to crop image.");
            }

            return this;
        }

        public ImageProcessorWrapper SetRotationCenter(float centerX, float centerY)
        {
            if (_processorHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException("图像处理器可能已经被释放了。无法获取图像处理器。");
            }

            if (SetRotationCenter(_processorHandle, centerX, centerY) != 0)
            {
                throw new Exception("设置旋转中心点错误");
            }

            return this;
        }

        public ImageProcessorWrapper Rotate(double angle)
        {
            if (_processorHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException("图像处理器可能已经被释放了。无法获取图像处理器。");
            }

            if (Rotate(_processorHandle, angle) != 0)
            {
                throw new Exception("旋转图像出错！");
            }

            return this;
        }

        public ImageProcessorWrapper Translate(double offsetX, double offsetY)
        {
            if (_processorHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException("图像处理器可能已经被释放了。无法获取图像处理器。");
            }

            if (Translate(_processorHandle, offsetX, offsetY) != 0)
            {
                throw new Exception("平移图像出错！");
            }

            return this;
        }

        public static Bitmap Overlay(Bitmap background, Bitmap overlay, int x, int y)
        {
            if (overlay.Width > background.Width || overlay.Height > background.Height)
            {
                throw new ArgumentException("要覆盖的图像比背景图大！请先考虑调整大小。");
            }
            // 创建一个新的Bitmap对象，大小与背景图像相同
            var result = new Bitmap(background.Width, background.Height);

            // 使用Graphics对象绘制背景图像
            using (var g = Graphics.FromImage(result))
            {
                g.DrawImage(background, 0, 0);  // 画背景
                g.DrawImage(overlay, x, y);  // 画覆盖图
            }

            return result;
        }

        public void SaveImage(string fileName)
        {
            using (Bitmap processedImage = this.GetProcessedImage())
            {
                processedImage.Save(fileName);
                Console.WriteLine($"Image saved to {fileName}");
            }
        }

        public void Dispose()
        {
            if (_processorHandle != IntPtr.Zero)
            {
                DestroyImageProcessor(_processorHandle);
                _processorHandle = IntPtr.Zero;
            }
        }
    }
}