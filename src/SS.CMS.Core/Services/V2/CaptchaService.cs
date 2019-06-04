using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Http;
using SS.CMS.Plugin;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services.V2
{
    public partial class CaptchaService : ServiceBase
    {
        public const string Route = "captcha/{name}";
        public const string RouteActionsCheck = "captcha/{name}/actions/check";

        public ResponseResult<byte[]> Get(IRequest request, IResponse response)
        {
            var name = TranslateUtils.Get<string>(request.RouteValues, "name");
            var cookieName = GetCookieName(name);
            var code = CreateValidateCode();
            if (IsCacheExists(name, code))
            {
                code = CreateValidateCode();
            }

            response.Cookies.Delete(cookieName);
            response.Cookies.Append(cookieName, code, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            });

            byte[] buffer;

            using (var image = new Bitmap(130, 53, PixelFormat.Format32bppRgb))
            {
                var r = new Random();
                var colors = Colors[r.Next(0, 5)];

                using (var g = Graphics.FromImage(image))
                {
                    g.FillRectangle(new SolidBrush(Color.FromArgb(240, 243, 248)), 0, 0, 200, 200); //矩形框
                    g.DrawString(code, new Font(FontFamily.GenericSerif, 28, FontStyle.Bold | FontStyle.Italic), new SolidBrush(colors), new PointF(14, 3));//字体/颜色

                    var random = new Random();

                    for (var i = 0; i < 25; i++)
                    {
                        var x1 = random.Next(image.Width);
                        var x2 = random.Next(image.Width);
                        var y1 = random.Next(image.Height);
                        var y2 = random.Next(image.Height);

                        g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                    }

                    for (var i = 0; i < 100; i++)
                    {
                        var x = random.Next(image.Width);
                        var y = random.Next(image.Height);

                        image.SetPixel(x, y, Color.FromArgb(random.Next()));
                    }

                    g.Save();
                }

                using (var ms = new MemoryStream())
                {
                    image.Save(ms, ImageFormat.Png);
                    buffer = ms.ToArray();
                }
            }

            return File(".png", buffer);
        }

        public ResponseResult<object> ActionsCheck(IRequest request, IResponse response)
        {
            var name = TranslateUtils.Get<string>(request.RouteValues, "name");
            var cookieName = GetCookieName(name);
            var captcha = request.GetPostString("captcha");

            request.Cookies.TryGetValue(cookieName, out var code);

            if (string.IsNullOrEmpty(code) || IsCacheExists(name, code))
            {
                return BadRequest("验证码已超时，请点击刷新验证码！");
            }

            response.Cookies.Delete(cookieName);
            Cache(name, code);

            if (!StringUtils.EqualsIgnoreCase(code, captcha))
            {
                return BadRequest("验证码不正确，请重新输入！");
            }

            return Ok(new
            {
                Value = true
            });
        }
    }
}
