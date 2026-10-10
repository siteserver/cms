using System;
using System.IO;
using SixLabors.Fonts;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Drawing.Processing;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Utils
{
    public static class CaptchaUtils
    {
        private static readonly Color[] Colors = { Color.FromRgb(37, 72, 91), Color.FromRgb(68, 24, 25), Color.FromRgb(17, 46, 2), Color.FromRgb(70, 16, 100), Color.FromRgb(24, 88, 74) };
        // private static readonly char[] Chars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private static readonly char[] Chars = { 'B','C','E','F','G','H','J','K','M','P','Q','R','T','V','W','X','Y','2','3','4','6','7','8','9' };

        public const int ExpireMinutes = 10;
        public const string ErrorInvalid = "验证码不正确或已超时，请刷新验证码后重新输入！";

        private static string GetServerKey(string token) =>
            CacheUtils.GetClassKey(typeof(CaptchaUtils), "Server", token);

        private static bool IsValidToken(string token) =>
            !string.IsNullOrEmpty(token) && token.Length == 32 && System.Text.RegularExpressions.Regex.IsMatch(token, "^[0-9a-f]{32}$");

        /// <summary>
        /// 服务端保存验证码（GJ20260924）：只向客户端返回随机令牌，验证码值保存在缓存中，
        /// 不再用 SecurityKey 加密后交给客户端往返，避免伪造与解密预言机。
        /// </summary>
        public static string Issue(ICacheManager cacheManager)
        {
            var token = Convert.ToHexString(System.Security.Cryptography.RandomNumberGenerator.GetBytes(16)).ToLowerInvariant();
            cacheManager.AddOrUpdateAbsolute(GetServerKey(token), GetCode(), ExpireMinutes);
            return token;
        }

        /// <summary>
        /// 获取验证码值用于绘制图片，不消费令牌；令牌无效返回 null。
        /// </summary>
        public static string GetValue(ICacheManager cacheManager, string token)
        {
            if (!IsValidToken(token)) return null;
            var value = cacheManager.Get<string>(GetServerKey(token));
            return string.IsNullOrEmpty(value) ? null : value;
        }

        /// <summary>
        /// 校验验证码。无论成功与否令牌都立即作废（一次性）。
        /// </summary>
        public static bool Validate(ICacheManager cacheManager, string token, string input)
        {
            var value = GetValue(cacheManager, token);
            if (value == null) return false;
            cacheManager.Remove(GetServerKey(token));
            if (string.IsNullOrEmpty(input)) return false;
            return SecureCompareUtils.FixedTimeEquals(value.ToUpperInvariant(), input.Trim().ToUpperInvariant());
        }

        public static string GetCode()
        {
            var code = string.Empty;

            for (var i = 0; i < 4; i++)
            {
                code += Chars[System.Security.Cryptography.RandomNumberGenerator.GetInt32(0, Chars.Length)].ToString();
            }

            return code;
        }

        public static byte[] GetCaptcha(string code)
        {
            byte[] buffer;

            const int width = 130;
            const int height = 53;
            var r = new Random();
            using (var image = new Image<Rgba32>(width, height))
            {
                var font = FontUtils.DefaultFont.CreateFont(40);

                image.Mutate(ctx =>
                {
                    var color = Color.FromRgb(240, 243, 248);
                    ctx.Fill(color);
                    ctx.DrawText(code, font, Colors[r.Next(0, 5)], new PointF(10, 10));

                    var random = new Random();

                    for (var i = 0; i < 25; i++)
                    {
                        var x1 = random.Next(width);
                        var x2 = random.Next(width);
                        var y1 = random.Next(height);
                        var y2 = random.Next(height);

                        ctx.DrawLine(Colors[r.Next(0, 5)], 1, new PointF(x1, y1), new PointF(x2, y2));

                        // ctx.DrawLines(new Pen(Colors[r.Next(0, 5)], 1), new PointF(x1, y1), new PointF(x2, y2));
                    }

                    for (var i = 0; i < 100; i++)
                    {
                        var x = random.Next(width);
                        var y = random.Next(height);

                        ctx.DrawLine(Colors[r.Next(0, 5)], 1, new PointF(x, y), new PointF(x + 1, y + 1));

                        // ctx.DrawLines(new Pen(Colors[r.Next(0, 5)], 1), new PointF(x, y), new PointF(x + 1, y + 1));
                    }
                });

                using var ms = new MemoryStream();
                image.Save(ms, PngFormat.Instance);
                buffer = ms.ToArray();
            }

            return buffer;
        }
    }
}
