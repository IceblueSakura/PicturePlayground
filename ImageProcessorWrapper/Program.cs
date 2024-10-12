using System;
using System.Drawing;

namespace ImageProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            // 请替换为您本地的图片路径
            const string imagePath = "./image/example.bmp";
            const string outputDirectory = "./image/output/";

            try
            {
                using (Bitmap bitmap = new Bitmap(imagePath))
                {
                    using (ImageProcessorWrapper processor = new ImageProcessorWrapper(bitmap))
                    {
                        // 演示缩放功能
                        Console.WriteLine("Scaling image...");
                        processor
                            .Scale(0.5, 0.5)
                            .SaveImage($"{outputDirectory}/scaling.bmp");

                        // 演示裁剪功能
                        Console.WriteLine("Cropping image...");
                        processor
                            .CropRectangle(10, 10, 100, 100)
                            .SaveImage($"{outputDirectory}/cropped.bmp");

                        // 演示旋转功能
                        Console.WriteLine("Rotating image...");
                        processor
                            .SetRotationCenter(50, 50)
                            .Rotate(45)
                            .SaveImage($"{outputDirectory}/rotation.jpg");

                        // 演示合并旋转后图像与原图
                        Console.WriteLine("Overlay image...");
                        var background = new Bitmap(imagePath); // 再加载一份原图用于背景
                        var overlay = processor.GetProcessedImage(); // 获取一份变换后的图像用于覆盖
                        processor
                            .Overlay(background, overlay, 100, 100) // 覆盖到左上角座标为100,100的位置
                            .SaveImage($"{outputDirectory}/overlay.bmp");

                        // 演示平移功能
                        Console.WriteLine("Translating image...");
                        processor
                            .Translate(20, 20)
                            .SaveImage($"{outputDirectory}/translated.bmp");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }
    }
}