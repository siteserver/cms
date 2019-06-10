using System;
using System.Drawing;
using Microsoft.AspNetCore.Http;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Plugin.Apis;
using SS.CMS.Utils;

namespace SS.CMS.Api.Controllers.Admin
{
    public partial class LoginController
    {
        private readonly Color[] _colors = { Color.FromArgb(37, 72, 91), Color.FromArgb(68, 24, 25), Color.FromArgb(17, 46, 2), Color.FromArgb(70, 16, 100), Color.FromArgb(24, 88, 74) };

        private readonly string _cookieName = "SS-" + nameof(LoginController);

        private static string CreateValidateCode()
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

        public string AdminLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = AdminManager.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = _accessTokenRepository.GetAccessToken(adminInfo.Id, adminInfo.UserName, TimeSpan.FromDays(Constants.AccessTokenExpireDays));

            AdminApi.Instance.Log(adminInfo.UserName, "管理员登录");

            if (isAutoLogin)
            {
                Response.Cookies.Delete(Constants.CookieAdminToken);
                Response.Cookies.Append(Constants.CookieAdminToken, accessToken, new CookieOptions
                {
                    Expires = expiresAt
                });
            }
            else
            {
                Response.Cookies.Delete(Constants.CookieAdminToken);
                Response.Cookies.Append(Constants.CookieAdminToken, accessToken);
            }

            return accessToken;
        }

        public void AdminLogout()
        {
            Response.Cookies.Delete(Constants.CookieAdminToken);
        }
    }
}
