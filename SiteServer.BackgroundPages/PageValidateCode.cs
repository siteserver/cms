using System;
using System.Collections.Specialized;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using System.Web.UI;
using BaiRong.Core;
using SiteServer.BackgroundPages.Core;

namespace SiteServer.BackgroundPages
{
    public class PageValidateCode : Page
    {
        private static readonly Color[] Colors = { Color.FromArgb(37, 72, 91), Color.FromArgb(68, 24, 25), Color.FromArgb(17, 46, 2), Color.FromArgb(70, 16, 100), Color.FromArgb(24, 88, 74) };

        public static string GetRedirectUrl(string cookieName)
        {
            return PageUtils.GetSiteServerUrl(nameof(PageValidateCode), new NameValueCollection
            {
                {"cookieName", cookieName}
            });
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var cookieName = Request.QueryString["cookieName"];

            var validateCode = VcManager.CreateValidateCode();

            CookieUtils.SetCookie(cookieName, validateCode, DateTime.Now.AddDays(1));

            Response.BufferOutput = true;  //特别注意
            Response.Cache.SetExpires(DateTime.Now.AddMilliseconds(-1));//特别注意
            Response.Cache.SetCacheability(HttpCacheability.NoCache);//特别注意
            Response.AppendHeader("Pragma", "No-Cache"); //特别注意
            Response.ContentType = "image/png";
            ValidateCode(validateCode);
        }

        public void ValidateCode(string validateCode)
        {
            var validateimage = new Bitmap(130, 53, PixelFormat.Format32bppRgb);

            var r = new Random();
            var colors = Colors[r.Next(0, 5)];

            var g = Graphics.FromImage(validateimage);
            g.FillRectangle(new SolidBrush(Color.FromArgb(240, 243, 248)), 0, 0, 200, 200); //矩形框
            g.DrawString(validateCode, new Font(FontFamily.GenericSerif, 28, FontStyle.Bold | FontStyle.Italic), new SolidBrush(colors), new PointF(14, 3));//字体/颜色

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
            var ms = new MemoryStream();
            validateimage.Save(ms, ImageFormat.Png);
            Response.ClearContent();
            Response.BinaryWrite(ms.ToArray());
            Response.End();
        }
    }
}
