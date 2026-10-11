using System;
using System.IO;
using SkiaSharp;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    /// <summary>
    /// SkiaSharp 图片读写与绘制的基础方法。
    /// </summary>
    public static partial class ImageUtils
    {
        private static readonly SKSamplingOptions HighQualitySampling = new SKSamplingOptions(SKCubicResampler.Mitchell);

        /// <summary>
        /// 读取图片文件，无法解码时抛出异常。
        /// </summary>
        public static SKBitmap Load(string imagePath)
        {
            return SKBitmap.Decode(imagePath) ?? throw new InvalidOperationException($"无法识别的图片格式：{imagePath}");
        }

        public static SKBitmap Load(byte[] data)
        {
            return SKBitmap.Decode(data) ?? throw new InvalidOperationException("无法识别的图片数据");
        }

        /// <summary>
        /// 读取图片尺寸，只解析文件头，不解码像素。
        /// </summary>
        public static (int width, int height) GetSize(string imagePath)
        {
            using var codec = SKCodec.Create(imagePath) ?? throw new InvalidOperationException($"无法识别的图片格式：{imagePath}");
            return (codec.Info.Width, codec.Info.Height);
        }

        /// <summary>
        /// 根据扩展名选择编码格式。Skia 只能编码 PNG / JPEG / WEBP，其他格式（如 GIF、BMP）返回 null。
        /// </summary>
        public static SKEncodedImageFormat? GetEncodedFormat(string imagePath)
        {
            var ext = PathUtils.GetExtension(imagePath)?.ToLowerInvariant();
            switch (ext)
            {
                case ".png":
                    return SKEncodedImageFormat.Png;
                case ".jpg":
                case ".jpeg":
                case ".jpe":
                case ".jfif":
                    return SKEncodedImageFormat.Jpeg;
                case ".webp":
                    return SKEncodedImageFormat.Webp;
                default:
                    return null;
            }
        }

        /// <summary>
        /// 按扩展名保存图片；格式不支持编码时抛出 NotSupportedException。
        /// </summary>
        public static void Save(SKBitmap bitmap, string imagePath)
        {
            var format = GetEncodedFormat(imagePath) ?? throw new NotSupportedException($"不支持保存为该图片格式：{imagePath}");
            Save(bitmap, imagePath, format);
        }

        public static void Save(SKBitmap bitmap, string imagePath, SKEncodedImageFormat format)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(format, 90) ?? throw new InvalidOperationException($"图片编码失败：{imagePath}");
            DirectoryUtils.CreateDirectoryIfNotExists(imagePath);
            // 先完整编码再写入，避免覆盖原图时写入一半失败导致原图损坏
            File.WriteAllBytes(imagePath, data.ToArray());
        }

        public static byte[] EncodePng(SKBitmap bitmap)
        {
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Png, 100);
            return data.ToArray();
        }

        public static SKBitmap Resize(SKBitmap bitmap, int width, int height)
        {
            var info = new SKImageInfo(Math.Max(1, width), Math.Max(1, height), bitmap.ColorType, bitmap.AlphaType);
            return bitmap.Resize(info, HighQualitySampling) ?? throw new InvalidOperationException("图片缩放失败");
        }

        /// <summary>
        /// 缩放图片文件并保存到目标路径。
        /// </summary>
        public static void ResizeTo(string originalFilePath, string resizeFilePath, int width, int height)
        {
            using var bitmap = Load(originalFilePath);
            using var resized = Resize(bitmap, width, height);
            Save(resized, resizeFilePath);
        }

        /// <summary>
        /// 顺时针旋转，画布扩展以容纳旋转后的完整图像（与 ImageSharp Rotate 行为一致）。
        /// </summary>
        private static SKBitmap Rotate(SKBitmap bitmap, float degrees)
        {
            var radians = degrees * Math.PI / 180;
            var cos = Math.Abs(Math.Cos(radians));
            var sin = Math.Abs(Math.Sin(radians));
            var newWidth = (int)Math.Ceiling(bitmap.Width * cos + bitmap.Height * sin);
            var newHeight = (int)Math.Ceiling(bitmap.Width * sin + bitmap.Height * cos);

            var rotated = new SKBitmap(new SKImageInfo(newWidth, newHeight, SKColorType.Rgba8888, SKAlphaType.Premul));
            using var canvas = new SKCanvas(rotated);
            canvas.Clear(SKColors.Transparent);
            canvas.Translate(newWidth / 2f, newHeight / 2f);
            canvas.RotateDegrees(degrees);
            canvas.Translate(-bitmap.Width / 2f, -bitmap.Height / 2f);
            using var image = SKImage.FromBitmap(bitmap);
            canvas.DrawImage(image, 0, 0, HighQualitySampling);
            return rotated;
        }

        /// <summary>
        /// 以左上角为原点绘制文字（Skia 默认以基线定位）。
        /// </summary>
        private static void DrawTextTopLeft(SKCanvas canvas, string text, float x, float y, SKFont font, SKColor color)
        {
            using var paint = new SKPaint { Color = color, IsAntialias = true };
            canvas.DrawText(text, x, y - font.Metrics.Ascent, font, paint);
        }
    }
}
