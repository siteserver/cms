using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using CacheManager.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Abstractions;
using SS.CMS.Abstractions.Dto.Result;
using SS.CMS.Web.Extensions;

namespace SS.CMS.Web.Controllers.V1
{
    [Route("v1/captcha")]
    public partial class CaptchaController : ControllerBase
    {
        private const string ApiRoute = "{name}";
        private const string ApiRouteActionsCheck = "{name}/actions/check";

        private readonly ICacheManager<bool> _cacheManager;

        public CaptchaController(ICacheManager<bool> cacheManager)
        {
            _cacheManager = cacheManager;
        }

        private static readonly Color[] Colors = { Color.FromArgb(37, 72, 91), Color.FromArgb(68, 24, 25), Color.FromArgb(17, 46, 2), Color.FromArgb(70, 16, 100), Color.FromArgb(24, 88, 74) };

        [HttpGet, Route(ApiRoute)]
        public FileResult Get(string name)
        {
            var code = string.Empty;

            char[] s = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            var r = new Random();
            for (var i = 0; i < 4; i++)
            {
                code += s[r.Next(0, s.Length)].ToString();
            }

            var cookieName = "SS-" + name;

            Response.Cookies.Delete(cookieName);
            Response.Cookies.Append(cookieName, code, new CookieOptions
            {
                Expires = DateTime.Now.AddMinutes(10)
            });

            byte[] buffer;

            using (var image = new Bitmap(130, 53, PixelFormat.Format32bppRgb))
            {
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

                using var ms = new MemoryStream();
                image.Save(ms, ImageFormat.Png);
                buffer = ms.ToArray();
            }

            return File(buffer, "image/png");
        }

        [HttpPost, Route(ApiRouteActionsCheck)]
        public ActionResult<BoolResult> Check(string name, [FromBody] CheckRequest checkRequest)
        {
            var cookieName = "SS-" + name;

            if (!HttpContext.Request.Cookies.TryGetValue(cookieName, out var code))
            {
                return this.Error("验证码已超时，请点击刷新验证码！");
            }

            var cacheKey = $"{cookieName}:{code}";

            if (_cacheManager.Exists(cacheKey))
            {
                return this.Error("验证码已超时，请点击刷新验证码！");
            }

            HttpContext.Response.Cookies.Delete(Constants.AuthKeyAdminCaptchaCookie);

            if (!StringUtils.EqualsIgnoreCase(code, checkRequest.Captcha))
            {
                return this.Error("验证码不正确，请重新输入！");
            }

            _cacheManager.Put(cacheKey, true);

            return new BoolResult
            {
                Value = true
            };
        }
    }
}
