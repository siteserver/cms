using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Http;
using SiteServer.BackgroundPages.Core;
using SiteServer.CMS.Core;
using SiteServer.Utils;

namespace SiteServer.API.Controllers.V1
{
    [RoutePrefix("v1/captcha")]
    public class V1CaptchaController : ApiController
    {
        private const string ApiRoute = "{name}";
        private const string ApiRouteActionsCheck = "{name}/actions/check";

        private static readonly Color[] Colors = { Color.FromArgb(37, 72, 91), Color.FromArgb(68, 24, 25), Color.FromArgb(17, 46, 2), Color.FromArgb(70, 16, 100), Color.FromArgb(24, 88, 74) };

        [HttpGet, Route(ApiRoute)]
        public void Get(string name)
        {
            var response = HttpContext.Current.Response;

            var code = VcManager.CreateValidateCode();
            if (CacheUtils.Exists($"SiteServer.API.Controllers.V1.CaptchaController.{code}"))
            {
                code = VcManager.CreateValidateCode();
            }

            CookieUtils.SetCookie("SS-" + name, code, TimeSpan.FromMinutes(10));

            response.BufferOutput = true;  //特别注意
            response.Cache.SetExpires(DateTime.Now.AddMilliseconds(-1));//特别注意
            response.Cache.SetCacheability(HttpCacheability.NoCache);//特别注意
            response.AppendHeader("Pragma", "No-Cache"); //特别注意
            response.ContentType = "image/png";

            var validateImage = new Bitmap(130, 53, PixelFormat.Format32bppRgb);

            var r = new Random();
            var colors = Colors[r.Next(0, 5)];

            var g = Graphics.FromImage(validateImage);
            g.FillRectangle(new SolidBrush(Color.FromArgb(240, 243, 248)), 0, 0, 200, 200); //矩形框
            g.DrawString(code, new Font(FontFamily.GenericSerif, 28, FontStyle.Bold | FontStyle.Italic), new SolidBrush(colors), new PointF(14, 3));//字体/颜色

            var random = new Random();

            for (var i = 0; i < 25; i++)
            {
                var x1 = random.Next(validateImage.Width);
                var x2 = random.Next(validateImage.Width);
                var y1 = random.Next(validateImage.Height);
                var y2 = random.Next(validateImage.Height);

                g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
            }

            for (var i = 0; i < 100; i++)
            {
                var x = random.Next(validateImage.Width);
                var y = random.Next(validateImage.Height);

                validateImage.SetPixel(x, y, Color.FromArgb(random.Next()));
            }

            g.Save();
            var ms = new MemoryStream();
            validateImage.Save(ms, ImageFormat.Png);
            response.ClearContent();
            response.BinaryWrite(ms.ToArray());
            response.End();
        }

        [HttpPost, Route(ApiRouteActionsCheck)]
        public IHttpActionResult Check(string name)
        {
            var rest = new Rest(Request);
            var captcha = rest.GetPostString("captcha");

            try
            {
                var code = CookieUtils.GetCookie("SS-" + name);

                if (string.IsNullOrEmpty(code) || CacheUtils.Exists($"SiteServer.API.Controllers.V1.CaptchaController.{code}"))
                {
                    return BadRequest("验证码已超时，请点击刷新验证码！");
                }

                CookieUtils.Erase("SS-" + name);
                CacheUtils.InsertMinutes($"SiteServer.API.Controllers.V1.CaptchaController.{code}", true, 10);

                if (!StringUtils.EqualsIgnoreCase(code, captcha))
                {
                    return BadRequest("验证码不正确，请重新输入！");
                }

                return Ok(new
                {
                    Value = true
                });
            }
            catch (Exception ex)
            {
                LogUtils.AddErrorLog(ex);
                return InternalServerError(ex);
            }
        }
    }
}
