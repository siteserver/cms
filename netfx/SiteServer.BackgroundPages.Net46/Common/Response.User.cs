using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.Utils;
using System;
using CookieManager = SiteServer.CMS.Core.CookieManager;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Response
    {
        public string UserLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userInfo = UserManager.GetUserInfoByUserName(userName);
            if (userInfo == null || userInfo.Locked || !userInfo.Checked) return null;

            var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
            var accessToken = UserApi.Instance.GetAccessToken(userInfo.Id, userInfo.UserName, TimeSpan.FromDays(Constants.AccessTokenExpireDays));

            DataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(userInfo);
            UserApi.Instance.Log(userInfo.UserName, "用户登录");

            if (isAutoLogin)
            {
                CookieManager.Set(CookieManager.UserToken, this, accessToken, expiresAt);
            }
            else
            {
                CookieManager.Set(CookieManager.UserToken, this, accessToken);
            }

            return accessToken;
        }

        public void UserLogout()
        {
            CookieManager.Delete(CookieManager.UserToken, this);
        }
    }
}
