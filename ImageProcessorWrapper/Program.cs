using System;
using System.Drawing;

namespace ImageProcessor
{
 class Program
    {
        static void Main(string[] args)
        {
            // 请替换为您本地的图片路径
            string imagePath = "./image/example.bmp";
            string outputDirectory = "./image/output/";

            try
            {
                using (Bitmap bitmap = new Bitmap(imagePath))
                {
                    using (ImageProcessorWrapper processor = new ImageProcessorWrapper(bitmap))
                    {
                        // 演示缩放功能
                        Console.WriteLine("Scaling image...");
                        processor.Scale(0.5, 0.5);
                        SaveOutputImage(processor, outputDirectory, "scaled.jpg");

                        // 演示裁剪功能
                        Console.WriteLine("Cropping image...");
                        processor.CropRectangle(10, 10, 100, 100);
                        SaveOutputImage(processor, outputDirectory, "cropped.jpg");

                        // 演示旋转功能
                        Console.WriteLine("Rotating image...");
                        processor.SetRotationCenter(50, 50);
                        processor.Rotate(45); // 旋转45度
                        SaveOutputImage(processor, outputDirectory, "rotated.jpg");

                        // 演示平移功能
                        Console.WriteLine("Translating image...");
                        processor.Translate(20, 20);
                        SaveOutputImage(processor, outputDirectory, "translated.jpg");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        private static void SaveOutputImage(ImageProcessorWrapper processor, string outputDirectory, string fileName)
        {
            using (Bitmap processedImage = processor.GetProcessedImage())
            {
                string outputPath = System.IO.Path.Combine(outputDirectory, fileName);
                processedImage.Save(outputPath);
                Console.WriteLine($"Image saved to {outputPath}");
            }
        }
    }
}