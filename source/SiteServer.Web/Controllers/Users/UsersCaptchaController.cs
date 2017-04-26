using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.Http;
using BaiRong.Core;
using SiteServer.CMS.Controllers.Users;

namespace SiteServer.API.Controllers.Users
{
    [RoutePrefix("api")]
    public class UsersCaptchaController : ApiController
    {
        private static readonly Color[] Colors = { Color.FromArgb(37, 72, 91), Color.FromArgb(68, 24, 25), Color.FromArgb(17, 46, 2), Color.FromArgb(70, 16, 100), Color.FromArgb(24, 88, 74) };

        [HttpGet]
        [Route(Captcha.Route)]
        public void Main(string code)
        {
            var ms = CacheUtils.Get($"users/captcha/{code}") as MemoryStream;

            if (ms == null)
            {
                var r = new Random();
                var randomColor = Colors[r.Next(0, 5)];

                var validateimage = new Bitmap(130, 53, PixelFormat.Format32bppRgb);

                var g = Graphics.FromImage(validateimage);
                g.FillRectangle(new SolidBrush(Color.FromArgb(240, 243, 248)), 0, 0, 200, 200); //矩形框
                g.DrawString(code, new Font(FontFamily.GenericSerif, 28, FontStyle.Bold | FontStyle.Italic), new SolidBrush(randomColor), new PointF(14, 3));//字体/颜色

                var random = new Random();

                for (var i = 0; i < 25; i++)
                {
                    var x1 = random.Next(validateimage.Width);
                    var x2 = random.Next(validateimage.Width);
                    var y1 = random.Next(validateimage.Height);
                    var y2 = random.Next(validateimage.Height);

                    g.DrawLine(new Pen(Color.Silver), x1, y1, x2, y2);
                }

                for (var i = 0; i < 100; i++)
                {
                    var x = random.Next(validateimage.Width);
                    var y = random.Next(validateimage.Height);

                    validateimage.SetPixel(x, y, Color.FromArgb(random.Next()));
                }

                g.Save();
                ms = new MemoryStream();
                validateimage.Save(ms, ImageFormat.Png);

                CacheUtils.Insert($"users/captcha/{code}", ms, 600);
            }

            HttpContext.Current.Response.BufferOutput = true;  //特别注意
            HttpContext.Current.Response.Cache.SetExpires(DateTime.Now.AddMilliseconds(-1));//特别注意
            HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.NoCache);//特别注意
            HttpContext.Current.Response.AppendHeader("Pragma", "No-Cache"); //特别注意
            HttpContext.Current.Response.ContentType = "image/png";

            HttpContext.Current.Response.ClearContent();
            HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            HttpContext.Current.Response.End();
        }
    }
}
