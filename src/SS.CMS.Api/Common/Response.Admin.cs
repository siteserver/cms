using System;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Plugin.Apis;
using SS.CMS.Utils;

namespace SS.CMS.Api.Common
{
    public partial class Response
    {
        public string AdminLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = AdminManager.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

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

        //public object AdminRedirectCheck(bool checkInstall = false, bool checkDatabaseVersion = false,
        //    bool checkLogin = false)
        //{
        //    var redirect = false;
        //    var redirectUrl = string.Empty;

        //    if (checkInstall && string.IsNullOrEmpty(AppSettings.ConnectionString))
        //    {
        //        redirect = true;
        //        redirectUrl = PageUtils.GetAdminUrl("Installer/");
        //    }
        //    else if (checkDatabaseVersion && ConfigManager.Instance.Initialized &&
        //             ConfigManager.Instance.DatabaseVersion != SystemManager.ProductVersion)
        //    {
        //        redirect = true;
        //        redirectUrl = PageUtils.GetAdminUrl("pageSyncDatabase.aspx");
        //    }
        //    else if (checkLogin && (!IsAdminLoggin || AdminInfo == null || AdminInfo.Locked))
        //    {
        //        redirect = true;
        //        redirectUrl = PageUtils.GetAdminUrl("pageLogin.cshtml");
        //    }

        //    if (redirect)
        //    {
        //        return new
        //        {
        //            Value = false,
        //            RedirectUrl = redirectUrl
        //        };
        //    }

        //    return null;
        //}
    }
}
