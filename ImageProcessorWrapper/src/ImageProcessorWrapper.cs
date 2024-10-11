// ImageProcessorWrapper.cs

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace ImageProcessor
{
    public class ImageProcessorWrapper : IDisposable
    {
        // 导入CreateImageProcessor
        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr CreateImageProcessor(IntPtr bitmapData, int width, int height, byte channel,
            bool useGPU);

        // 导入DestroyImageProcessor
        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern void DestroyImageProcessor(IntPtr processor);

        // 导入AdjustBrightnessContrast
        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int AdjustBrightnessContrast(IntPtr processor, int brightness, double contrast);

        // 导入GetProcessedImage
        [DllImport("ImageProcessorLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int GetProcessedImage(IntPtr processor, out IntPtr data, out int width, out int height,
            out int channels);

        // 句柄
        private IntPtr _processorHandle;

        public ImageProcessorWrapper(Bitmap bitmap, bool useGpu = true)
        {
            if (bitmap.PixelFormat != PixelFormat.Format24bppRgb)
            {
                throw new ArgumentException("目前只支持24bbp,3 channel的图像");
            }

            // 锁定位图并获取指针
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadWrite,
                bitmap.PixelFormat);

            try
            {
                _processorHandle = CreateImageProcessor(bitmapData.Scan0, bitmap.Width, bitmap.Height, 3, useGpu);
                if (_processorHandle == IntPtr.Zero)
                    throw new Exception("Failed to create ImageProcessor");
            }
            finally
            {
                // cpp中已经深拷贝了一份内存数据，这里可以更快解锁保持UI不卡死了，但如果期间使用其他方式修改bitmap也会被覆盖
                bitmap.UnlockBits(bitmapData);
            }
        }

        // 调整亮度与对比度
        public ImageProcessorWrapper AdjustBrightnessContrast(int brightness, double contrast)
        {
            if (_processorHandle == IntPtr.Zero)
                throw new ObjectDisposedException("ImageProcessorWrapper");

            int result = AdjustBrightnessContrast(_processorHandle, brightness, contrast);
            if (result != 0)
                throw new Exception("AdjustBrightnessContrast failed");

            return this; // 允许链式调用
        }

        // 获取处理后的图像
        public Bitmap GetProcessedImage()
        {
            if (_processorHandle == IntPtr.Zero)
            {
                throw new ObjectDisposedException("ImageProcessorWrapper");
            }

            var result = GetProcessedImage(_processorHandle, out var dataPtr,  // 调用cpp获取处理后的图像，并获取处理结果
                out var width, out var height, out var channels);
            if (result != 0)  // cpp返回的状态表示处理失败了
                throw new Exception("GetProcessedImage failed"); 
            try
            {
                // 根据通道数和位深创建 Bitmap
                var pixelFormat = PixelFormat.Format24bppRgb;
                if (channels == 4)
                    pixelFormat = PixelFormat.Format32bppArgb;

                var processedBitmap = new Bitmap(width, height, pixelFormat);

                var bitmapData = processedBitmap.LockBits(new Rectangle(0, 0, width, height),  // 锁定Bitmap的数据
                    ImageLockMode.WriteOnly,
                    pixelFormat);
                try
                {
                    int bytes = Math.Abs(bitmapData.Stride) * height;
                    Marshal.Copy(dataPtr, 0, bitmapData.Scan0, bytes);
                }
                finally
                {
                    processedBitmap.UnlockBits(bitmapData);
                }

                return processedBitmap;
            }
        }

        // 释放资源
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