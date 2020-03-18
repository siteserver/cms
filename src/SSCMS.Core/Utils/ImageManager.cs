using System;
using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SixLabors.Primitives;

namespace SSCMS.Core.Utils
{
    public static class ImageManager
    {
        private static readonly Color[] Colors = { Color.FromRgb(37, 72, 91), Color.FromRgb(68, 24, 25), Color.FromRgb(17, 46, 2), Color.FromRgb(70, 16, 100), Color.FromRgb(24, 88, 74) };
        private static readonly char[] Chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };

        public static (string, byte[]) GetCaptcha()
        {
            var code = string.Empty;

            var r = new Random();
            for (var i = 0; i < 4; i++)
            {
                code += Chars[r.Next(0, Chars.Length)].ToString();
            }

            byte[] buffer;

            const int width = 130;
            const int height = 53;
            using (var image = new Image<Rgba32>(width, height))
            {
                var font = FontManager.Fonts.CreateFont(FontManager.DefaultFont.Name, 40);

                image.Mutate(ctx =>
                {
                    ctx.Fill(Color.FromRgb(240, 243, 248));
                    ctx.DrawText(code, font, Colors[r.Next(0, 5)], new PointF(10, 10));

                    var random = new Random();

                    for (var i = 0; i < 25; i++)
                    {
                        var x1 = random.Next(width);
                        var x2 = random.Next(width);
                        var y1 = random.Next(height);
                        var y2 = random.Next(height);

                        ctx.DrawLines(new Pen(Colors[r.Next(0, 5)], 1), new PointF(x1, y1), new PointF(x2, y2));
                    }

                    for (var i = 0; i < 100; i++)
                    {
                        var x = random.Next(width);
                        var y = random.Next(height);

                        ctx.DrawLines(new Pen(Colors[r.Next(0, 5)], 1), new PointF(x, y), new PointF(x + 1, y + 1));
                    }
                });

                using var ms = new MemoryStream();
                image.Save(ms, PngFormat.Instance);
                buffer = ms.ToArray();
            }

            return (code, buffer);
        }
    }
}
