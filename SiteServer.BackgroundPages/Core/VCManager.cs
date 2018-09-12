using System;
using SiteServer.Utils;

namespace SiteServer.BackgroundPages.Core
{
    public class VcManager
    {
        private const string AdminLoginCookieName = "SITESERVER.VC.ADMINLOGIN";

        private string _cookieName;

        public static VcManager GetInstance()
        {
            var vc = new VcManager {_cookieName = AdminLoginCookieName};
            return vc;
        }

        public string GetCookieName()
        {
            return _cookieName;
        }

        public static string CreateValidateCode()
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

        public bool IsCodeValid(string validateCode)
        {
            var code = CookieUtils.GetCookie(_cookieName);
            var isValid = StringUtils.EqualsIgnoreCase(code, validateCode);

            if (isValid)
            {
                CacheUtils.Remove(_cookieName);
            }

            return isValid;
        }
    }
}
