using System;
using System.IO;
using SkiaSharp;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static partial class ImageUtils
    {
        public static void AddTextWaterMark(string imagePath, string text, string fontName, int fontSize, int waterMarkPosition, int waterMarkOpacity, int minWidth, int minHeight)
        {
            try
            {
                // 不支持编码的格式（如 GIF）直接跳过，避免破坏原图
                if (GetEncodedFormat(imagePath) == null) return;

                using (var img = Load(imagePath))
                {
                    if (minWidth > 0 && img.Width < minWidth) return;
                    if (minHeight > 0 && img.Height < minHeight) return;

                    using (var canvas = new SKCanvas(img))
                    using (var font = new SKFont(FontUtils.GetFont(fontName), fontSize))
                    {
                        ApplyTextWaterMark(canvas, new SKSizeI(img.Width, img.Height), font, text, waterMarkPosition, waterMarkOpacity);
                    }
                    Save(img, imagePath);
                }
            }
            catch
            {
                // ignored
            }
        }

        public static void AddImageWaterMark(string imagePath, string waterMarkImagePath, int waterMarkPosition, int waterMarkTransparency, int minWidth, int minHeight)
        {
            try
            {
                if (GetEncodedFormat(imagePath) == null) return;

                using (var img = Load(imagePath))
                {
                    if (minWidth > 0 && img.Width < minWidth) return;
                    if (minHeight > 0 && img.Height < minHeight) return;

                    using (var canvas = new SKCanvas(img))
                    {
                        ApplyImageWaterMark(canvas, new SKSizeI(img.Width, img.Height), waterMarkImagePath, waterMarkPosition, waterMarkTransparency);
                    }
                    Save(img, imagePath);
                }
            }
            catch
            {
                // ignored
            }
        }

        public static void Save(byte[] imgByte, string imagePath)
        {
            // 先解码以确认是合法图片
            using (var img = Load(imgByte))
            {
                var format = GetEncodedFormat(imagePath);
                if (format.HasValue)
                {
                    Save(img, imagePath, format.Value);
                }
                else
                {
                    // Skia 无法编码的格式（GIF、BMP 等）按原始字节保存
                    DirectoryUtils.CreateDirectoryIfNotExists(imagePath);
                    File.WriteAllBytes(imagePath, imgByte);
                }
            }
        }

        public static void ResizeImageIfExceeding(string imagePath, int resizeWidth)
        {
            if (string.IsNullOrEmpty(imagePath) || resizeWidth <= 0) return;

            try
            {
                if (GetEncodedFormat(imagePath) == null) return;

                var (width, imgHeight) = GetSize(imagePath);
                if (width <= resizeWidth) return;

                var height = imgHeight * resizeWidth / width;
                ResizeTo(imagePath, imagePath, resizeWidth, height);
            }
            catch
            {
                // ignored
            }
        }


        private static void ApplyTextWaterMark(SKCanvas canvas, SKSizeI imgSize, SKFont font, string text,
            int waterMarkPosition, int waterMarkTransparency)
        {
            var pointF = GetWaterMarkPointF(imgSize, waterMarkPosition, 300, 60, true);
            // 与原实现一致：透明度 1-9 对应 alpha 0x10-0x90，10 为不透明
            var alpha = waterMarkTransparency == 10 ? (byte)255 : Convert.ToByte($"{waterMarkTransparency}0", 16);
            DrawTextTopLeft(canvas, text, pointF.X, pointF.Y, font, new SKColor(255, 255, 255, alpha));
        }

        private static void ApplyImageWaterMark(SKCanvas canvas, SKSizeI imgSize, string waterMarkImagePath,
            int waterMarkPosition, int waterMarkTransparency)
        {
            using (var wmImg = Load(waterMarkImagePath))
            using (var wmImage = SKImage.FromBitmap(wmImg))
            {
                var pointF = GetWaterMarkPointF(imgSize, waterMarkPosition, wmImg.Width, wmImg.Height, false);
                var opacity = Math.Clamp(TranslateUtils.ToFloat(waterMarkTransparency.ToString()) / 10, 0f, 1f);
                using var paint = new SKPaint { Color = SKColors.White.WithAlpha((byte)Math.Round(opacity * 255)) };
                canvas.DrawImage(wmImage, Convert.ToInt32(pointF.X), Convert.ToInt32(pointF.Y), HighQualitySampling, paint);
            }
        }

        private static SKPoint GetWaterMarkPointF(SKSizeI image, int waterMarkPosition, float waterMarkWidth, float waterMarkHeight, bool textMark)
        {
            float x;
            float y;
            switch (waterMarkPosition)
            {
                case 1:
                    if (textMark)
                    {
                        x = waterMarkWidth / 2;
                    }
                    else
                    {
                        x = 0;
                    }
                    y = 0;
                    break;
                case 2:
                    if (textMark)
                    {
                        x = (image.Width / 2);
                    }
                    else
                    {
                        x = (image.Width / 2) - (waterMarkWidth / 2);
                    }
                    y = 0;
                    break;
                case 3:
                    if (textMark)
                    {
                        x = image.Width - waterMarkWidth / 2;
                    }
                    else
                    {
                        x = image.Width - waterMarkWidth;
                    }
                    y = 0;
                    break;
                case 4:
                    if (textMark)
                    {
                        x = waterMarkWidth / 2;
                    }
                    else
                    {
                        x = 0;
                    }
                    y = (image.Height / 2) - (waterMarkHeight / 2);
                    break;
                case 5:
                    if (textMark)
                    {
                        x = (image.Width / 2);
                    }
                    else
                    {
                        x = (image.Width / 2) - (waterMarkWidth / 2);
                    }
                    y = (image.Height / 2) - (waterMarkHeight / 2);
                    break;
                case 6:
                    if (textMark)
                    {
                        x = image.Width - waterMarkWidth / 2;
                    }
                    else
                    {
                        x = image.Width - waterMarkWidth;
                    }
                    y = (image.Height / 2) - (waterMarkHeight / 2);
                    break;
                case 7:
                    if (textMark)
                    {
                        x = waterMarkWidth / 2;
                    }
                    else
                    {
                        x = 0;
                    }
                    y = image.Height - waterMarkHeight;
                    break;
                case 8:
                    if (textMark)
                    {
                        x = (image.Width / 2);
                    }
                    else
                    {
                        x = (image.Width / 2) - (waterMarkWidth / 2);
                    }
                    y = image.Height - waterMarkHeight;
                    break;
                default:

                    if (textMark)
                    {
                        x = image.Width - waterMarkWidth / 2;
                    }
                    else
                    {
                        x = image.Width - waterMarkWidth;
                    }
                    y = image.Height - waterMarkHeight;
                    break;
            }
            return new SKPoint(x, y);
        }

        public static void MakeThumbnail(string originalImagePath, string thumbnailPath, int width, int height, bool isLessSizeNotThumb)
        {
            if (!FileUtils.IsFileExists(originalImagePath))
            {
                return;
            }
            if (width == 0 && height == 0)
            {
                FileUtils.CopyFile(originalImagePath, thumbnailPath);
                return;
            }
            DirectoryUtils.CreateDirectoryIfNotExists(thumbnailPath);

            using (var originalImage = Load(originalImagePath))
            {
                if (width == 0)
                {
                    if (isLessSizeNotThumb && originalImage.Height < height)
                    {
                        FileUtils.CopyFile(originalImagePath, thumbnailPath);
                        return;
                    }
                    MakeThumbnail(originalImage, originalImagePath, thumbnailPath, width, height, "H");
                }
                else if (height == 0)
                {
                    if (isLessSizeNotThumb && originalImage.Width < width)
                    {
                        FileUtils.CopyFile(originalImagePath, thumbnailPath);
                        return;
                    }
                    MakeThumbnail(originalImage, originalImagePath, thumbnailPath, width, height, "W");
                }
                else if (isLessSizeNotThumb && originalImage.Height < height && originalImage.Width < width)
                {
                    FileUtils.CopyFile(originalImagePath, thumbnailPath);
                }
                else
                {
                    MakeThumbnail(originalImage, originalImagePath, thumbnailPath, width, height, "HW");
                }
            }
        }

        private static void MakeThumbnail(SKBitmap originalImage, string originalImagePath, string thumbnailPath, int width, int height, string mode)
        {
            var toWidth = width;
            var toHeight = height;
            var x = 0;
            var y = 0;
            var ow = originalImage.Width;
            var oh = originalImage.Height;
            switch (mode)
            {
                case "HW":
                    break;
                case "W":
                    toHeight = originalImage.Height * width / originalImage.Width;
                    break;
                case "H":
                    toWidth = originalImage.Width * height / originalImage.Height;
                    break;
                case "Cut":
                    if ((double)originalImage.Width / originalImage.Height > toWidth / (double)toHeight)
                    {
                        oh = originalImage.Height;
                        ow = originalImage.Height * toWidth / toHeight;
                        y = 0;
                        x = (originalImage.Width - ow) / 2;
                    }
                    else
                    {
                        ow = originalImage.Width;
                        oh = originalImage.Width * height / toWidth;
                        x = 0;
                        y = (originalImage.Height - oh) / 2;
                    }
                    break;
            }

            try
            {
                using var resized = Resize(originalImage, toWidth, toHeight);
                Save(resized, thumbnailPath);
            }
            catch
            {
                FileUtils.CopyFile(originalImagePath, thumbnailPath);
            }
        }

        public static string GetBackgroundWatermark(string text)
        {
            byte[] buffer;

            using (var image = new SKBitmap(new SKImageInfo(200, 200, SKColorType.Rgba8888, SKAlphaType.Premul)))
            {
                using (var canvas = new SKCanvas(image))
                using (var font = new SKFont(FontUtils.GetFont("simhei"), 17))
                {
                    canvas.Clear(SKColors.Transparent);
                    DrawTextTopLeft(canvas, text ?? string.Empty, 0, 0, font, SKColors.Black);
                }

                using (var rotated = Rotate(image, 315))
                {
                    buffer = EncodePng(rotated);
                }
            }

            return Convert.ToBase64String(buffer);
        }
    }
}
