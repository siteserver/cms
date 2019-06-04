using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.Utils;
using System;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Response
    {
        public string AdminLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = AdminManager.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            //var expiresAt = TimeSpan.FromDays(Constants.AccessTokenExpireDays);
            var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = AdminApi.Instance.GetAccessToken(adminInfo.Id, adminInfo.UserName, TimeSpan.FromDays(Constants.AccessTokenExpireDays));

            AdminApi.Instance.Log(adminInfo.UserName, "管理员登录");

            if (isAutoLogin)
            {
                CookieManager.Set(CookieManager.AdminToken, this, accessToken, expiresAt);
            }
            else
            {
                CookieManager.Set(CookieManager.AdminToken, this, accessToken);
            }

            return accessToken;
        }

        public void AdminLogout()
        {
            CookieManager.Delete(CookieManager.AdminToken, this);
        }
    }
}
