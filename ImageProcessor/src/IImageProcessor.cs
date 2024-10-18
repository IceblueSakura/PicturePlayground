using System.Drawing;
using OpenCvSharp;

namespace ImageProcessor
{
    /// <summary>
    /// 在背景图上加载前景图，并对前景图进行变换
    ///  _backgroundImage 背景图(将不做修改)
    /// _foregroundImage 前景图(前景图的修改，如旋转，将直接把结果应用修改到此成员变量上)
    /// _currentForegroundLocation 当前前景图在背景图的座标
    /// </summary>
    public interface IImageProcessor
    {
        /// <summary>
        /// 加载背景图，背景图在加载后不会进行任何变换或修改。
        /// </summary>
        /// <param name="image">Mat 格式的背景图</param>
        /// <returns>当前 IImageProcessor 实例，以便链式调用</returns>
        IImageProcessor LoadBackground(Mat image);

        /// <summary>
        /// 加载背景图，背景图在加载后不会进行任何变换或修改。通过 Bitmap.ToMat 方法将 Bitmap 转换为 Mat 格式。
        /// </summary>
        /// <param name="image">Bitmap 格式的背景图</param>
        /// <returns>当前 IImageProcessor 实例，以便链式调用</returns>
        IImageProcessor LoadBackground(Bitmap image);

        /// <summary>
        /// 加载前景图，前景图在加载后可能会进行变换操作。
        /// </summary>
        /// <param name="image">Mat 格式的前景图</param>
        /// <returns>当前 IImageProcessor 实例，以便链式调用</returns>
        IImageProcessor LoadForeground(Mat image);

        /// <summary>
        /// 加载前景图，前景图在加载后可能会进行变换操作。通过 Bitmap.ToMat 方法将 Bitmap 转换为 Mat 格式。
        /// </summary>
        /// <param name="image">Bitmap 格式的前景图</param>
        /// <returns>当前 IImageProcessor 实例，以便链式调用</returns>
        IImageProcessor LoadForeground(Bitmap image);

        /// <summary>
        /// 将前景图覆盖到背景图的指定位置，并更新 _processedImage。
        /// </summary>
        /// <returns>当前 IImageProcessor 实例，以便链式调用</returns>
        IImageProcessor Overlay();


        /// <summary>
        /// 将前景图平移，空白处使用0填充。平移操作会更新 _currentForegroundLocation，并确保平移后的前景图不会超出背景图边界。
        /// </summary>
        /// <param name="offsetX">X轴方向的平移像素数</param>
        /// <param name="offsetY">Y轴方向的平移像素数</param>
        /// <returns>当前 IImageProcessor 实例，以便链式调用</returns>
        IImageProcessor Translate(int offsetX, int offsetY);


        /// <summary>
        /// 将前景图按比例缩放，缩放操作会更新 _foregroundImage。
        /// </summary>
        /// <param name="scaleX">X轴方向的缩放比例</param>
        /// <param name="scaleY">Y轴方向的缩放比例</param>
        /// <returns>当前 IImageProcessor 实例，以便链式调用</returns>
        IImageProcessor Scale(double scaleX, double scaleY);


        /// <summary>
        /// 从前景图中裁切一块矩形区域，裁剪操作会更新 _foregroundImage。
        /// </summary>
        /// <param name="startX">裁剪区域左上角的X坐标</param>
        /// <param name="startY">裁剪区域左上角的Y坐标</param>
        /// <param name="width">裁剪区域的宽度</param>
        /// <param name="height">裁剪区域的高度</param>
        /// <returns>当前 IImageProcessor 实例，以便链式调用</returns>
        IImageProcessor CropRectangle(int startX, int startY, int width, int height);

        /// <summary>
        /// 将前景图旋转任意角度，旋转导致的空白处使用白色(255)填充。旋转操作会更新 _foregroundImage。
        /// </summary>
        /// <param name="angle">旋转角度</param>
        /// <returns>当前 IImageProcessor 实例，以便链式调用</returns>
        IImageProcessor Rotate(double angle);

        /// <summary>
        /// 获取处理后的 Mat 图像。在这一步会调用 Overlay 方法合并处理后的前景图与背景图。
        /// </summary>
        /// <returns>处理后的 Mat 图像</returns>
        Mat GetMat();


        /// <summary>
        /// 获取处理后的 Bitmap 图像。这一步会调用 GetMat 方法得到 Mat 结果后再转换为 Bitmap。
        /// </summary>
        /// <returns>处理后的 Bitmap 图像</returns>
        Bitmap GetBitmap();
    }
}