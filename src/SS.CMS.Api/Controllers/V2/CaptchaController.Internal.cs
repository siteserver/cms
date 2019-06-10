using System;
using System.Drawing;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.V2
{
    public partial class CaptchaController
    {
        private readonly Lazy<Color[]> Lazy = new Lazy<Color[]>(() => new[]
        {
            Color.FromArgb(37, 72, 91),
            Color.FromArgb(68, 24, 25),
            Color.FromArgb(17, 46, 2),
            Color.FromArgb(70, 16, 100),
            Color.FromArgb(24, 88, 74)
        });

        private Color[] Colors => Lazy.Value;

        private string GetCookieName(string name)
        {
            return "SS-CAPTCHA-" + name.ToUpper();
        }

        private string CreateValidateCode()
        {
            var validateCode = "";

            char[] s = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
            var r = new Random();
            for (var i = 0; i < 4; i++)
            {
                validateCode += s[r.Next(0, s.Length)].ToString();
            }

            return validateCode;
        }

        private bool IsCacheExists(string name, string code)
        {
            return CacheUtils.Exists($"SiteServer.CMS.Services.V2.CaptchaService.{name}.{code}");
        }

        private void Cache(string name, string code)
        {
            CacheUtils.InsertMinutes($"SiteServer.CMS.Services.V2.CaptchaService.{name}.{code}", true, 10);
        }
    }
}
