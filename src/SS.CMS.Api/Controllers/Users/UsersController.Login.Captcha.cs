using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Users
{
    public partial class UsersController
    {
        /// <summary>
        /// Get Login Form Captcha Image
        /// </summary>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /users/login/captcha
        ///
        /// </remarks>
        /// <returns>Captcha image</returns>
        /// <response code="200">Returns the image for login form</response>
        [ProducesResponseType(200)]
        [HttpGet("login/captcha")]
        public ActionResult GetLoginCaptcha()
        {
            var code = CreateValidateCode();

            Response.Cookies.Delete(_cookieName);
            Response.Cookies.Append(_cookieName, code, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(10)
            });

            byte[] buffer;

            using (var image = new Bitmap(130, 53, PixelFormat.Format32bppRgb))
            {
                var r = new Random();
                var colors = _colors[r.Next(0, 5)];

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

            if (!ContentTypeUtils.TryGetContentType(".png", out var contentType))
            {
                contentType = ContentTypeUtils.ContentTypeAttachment;
            }

            return File(buffer, contentType);
        }
    }
}