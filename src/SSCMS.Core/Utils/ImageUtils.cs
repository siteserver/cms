using System;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Processing;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class ImageUtils
    {
        public static void AddTextWaterMark(string imagePath, string text, string fontName, int fontSize, int waterMarkPosition, int waterMarkOpacity, int minWidth, int minHeight)
        {
            try
            {
                using (var img = Image.Load(imagePath))
                {
                    if (minWidth > 0)
                    {
                        if (img.Width < minWidth)
                        {
                            return;
                        }
                    }
                    if (minHeight > 0)
                    {
                        if (img.Height < minHeight)
                        {
                            return;
                        }
                    }

                    var font = FontUtils.GetFont(fontName).CreateFont(fontSize);

                    using (var img2 = img.Clone(ctx => ctx.ApplyTextWaterMark(font, text, waterMarkPosition, waterMarkOpacity)))
                    {
                        img2.Save(imagePath);
                    }
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
                using (var img = Image.Load(imagePath))
                {
                    if (minWidth > 0)
                    {
                        if (img.Width < minWidth)
                        {
                            return;
                        }
                    }
                    if (minHeight > 0)
                    {
                        if (img.Height < minHeight)
                        {
                            return;
                        }
                    }

                    using (var img2 = img.Clone(ctx => ctx.ApplyImageWaterMark(waterMarkImagePath, waterMarkPosition, waterMarkTransparency)))
                    {
                        img2.Save(imagePath);
                    }
                }
            }
            catch
            {
                // ignored
            }
        }

        public static void Save(byte[] imgByte, string imagePath)
        {
            using (var img = Image.Load(imgByte))
            {
                img.Save(imagePath);
            }
        }

        public static void ResizeImageIfExceeding(string imagePath, int resizeWidth)
        {
            if (string.IsNullOrEmpty(imagePath) || resizeWidth <= 0) return;

            try
            {
                using (var img = Image.Load(imagePath))
                {
                    if (img.Width <= resizeWidth)
                    {
                        return;
                    }

                    var height = img.Height * resizeWidth / img.Width;

                    img.Mutate(x => x.Resize(resizeWidth, height));
                    img.Save(imagePath);
                }
            }
            catch
            {
                // ignored
            }
        }


        private static IImageProcessingContext ApplyTextWaterMark(this IImageProcessingContext processingContext,
            Font font,
            string text,
            int waterMarkPosition,
            int waterMarkTransparency)
        {
            Size imgSize = processingContext.GetCurrentSize();
            var pointF = GetWaterMarkPointF(imgSize, waterMarkPosition, 300, 60, true);
            var a = waterMarkTransparency == 10 ? string.Empty : $"{waterMarkTransparency}0";
            var color = Color.Parse("ffffff" + a);
            return processingContext.DrawText(text, font, color, pointF);
        }

        private static IImageProcessingContext ApplyImageWaterMark(this IImageProcessingContext processingContext,
            string waterMarkImagePath,
            int waterMarkPosition,
            int waterMarkTransparency)
        {
            Size imgSize = processingContext.GetCurrentSize();

            using (var wmImg = Image.Load(waterMarkImagePath))
            {
                var pointF = GetWaterMarkPointF(imgSize, waterMarkPosition, wmImg.Width, wmImg.Height, false);
                var opacity = TranslateUtils.ToFloat(waterMarkTransparency.ToString()) / 10;
                return processingContext.DrawImage(wmImg, new Point(Convert.ToInt32(pointF.X), Convert.ToInt32(pointF.Y)), opacity);
            }
        }

        private static PointF GetWaterMarkPointF(Size image, int waterMarkPosition, float waterMarkWidth, float waterMarkHeight, bool textMark)
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
            return new PointF(x, y);
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

            using (var originalImage = Image.Load(originalImagePath))
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

        private static void MakeThumbnail(Image originalImage, string originalImagePath, string thumbnailPath, int width, int height, string mode)
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
                originalImage.Mutate(x => x.Resize(toWidth, toHeight));
                originalImage.Save(thumbnailPath);
            }
            catch
            {
                FileUtils.CopyFile(originalImagePath, thumbnailPath);
            }

        }
    }
}
