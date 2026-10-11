using System;
using System.IO;
using SkiaSharp;
using SSCMS.Core.Utils;
using Xunit;

namespace SSCMS.Core.Tests.Utils
{
    public class ImageUtilsTests : IDisposable
    {
        private readonly string _dir = Path.Combine(Path.GetTempPath(), "sscms-img-" + Guid.NewGuid().ToString("N"));

        public ImageUtilsTests() => Directory.CreateDirectory(_dir);

        public void Dispose() => Directory.Delete(_dir, true);

        private string CreateImage(string name, int width, int height, SKEncodedImageFormat format, SKColor? color = null)
        {
            var path = Path.Combine(_dir, name);
            using var bitmap = new SKBitmap(width, height);
            bitmap.Erase(color ?? SKColors.Red);
            ImageUtils.Save(bitmap, path, format);
            return path;
        }

        private static bool IsPng(byte[] data) =>
            data.Length > 8 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47;

        [Fact]
        public void Captcha_Is_Png()
        {
            var bytes = CaptchaUtils.GetCaptcha("AB23");
            Assert.True(IsPng(bytes));
            using var bmp = SKBitmap.Decode(bytes);
            Assert.Equal(130, bmp.Width);
            Assert.Equal(53, bmp.Height);
        }

        [Fact]
        public void Background_Watermark_Is_Png_Base64()
        {
            var bytes = Convert.FromBase64String(ImageUtils.GetBackgroundWatermark("管理员"));
            Assert.True(IsPng(bytes));
        }

        [Fact]
        public void Fonts_Are_Loaded()
        {
            var families = FontUtils.GetFontFamilies();
            Assert.Contains(families, x => x.Equals("Arial", StringComparison.OrdinalIgnoreCase));
            Assert.Contains(families, x => x.Equals("SimHei", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void ResizeImageIfExceeding_Keeps_Aspect_Ratio()
        {
            var path = CreateImage("a.jpg", 400, 200, SKEncodedImageFormat.Jpeg);
            ImageUtils.ResizeImageIfExceeding(path, 100);
            Assert.Equal((100, 50), ImageUtils.GetSize(path));
        }

        [Fact]
        public void MakeThumbnail_By_Width()
        {
            var path = CreateImage("b.png", 300, 150, SKEncodedImageFormat.Png);
            var thumb = Path.Combine(_dir, "b_s.png");
            ImageUtils.MakeThumbnail(path, thumb, 60, 0, true);
            Assert.Equal((60, 30), ImageUtils.GetSize(thumb));
        }

        [Fact]
        public void Text_And_Image_WaterMark_Modify_Image()
        {
            var path = CreateImage("c.png", 600, 400, SKEncodedImageFormat.Png);
            var before = File.ReadAllBytes(path);
            ImageUtils.AddTextWaterMark(path, "SSCMS", "Arial", 24, 5, 10, 0, 0);
            Assert.NotEqual(before, File.ReadAllBytes(path));

            var wm = CreateImage("wm.png", 50, 50, SKEncodedImageFormat.Png, SKColors.Blue);
            before = File.ReadAllBytes(path);
            ImageUtils.AddImageWaterMark(path, wm, 9, 5, 0, 0);
            Assert.NotEqual(before, File.ReadAllBytes(path));
            Assert.Equal((600, 400), ImageUtils.GetSize(path));
        }
    }
}
