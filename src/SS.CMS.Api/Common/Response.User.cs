using System;
using SS.CMS.Core.Cache;
using SS.CMS.Core.Common;
using SS.CMS.Core.Plugin.Apis;
using SS.CMS.Utils;

namespace SS.CMS.Api.Common
{
    public partial class Response
    {
        // public string UserLogin(string userName, bool isAutoLogin)
        // {
        //     if (string.IsNullOrEmpty(userName)) return null;

        //     var userInfo = UserManager.GetUserInfoByUserName(userName);
        //     if (userInfo == null || userInfo.Locked || !userInfo.Checked) return null;

        //     var expiresAt = DateTimeOffset.UtcNow.AddDays(Constants.AccessTokenExpireDays);
        //     var accessToken = accessTokenRepository.GetAccessToken(userInfo.Id, userInfo.UserName, TimeSpan.FromDays(Constants.AccessTokenExpireDays));

        //     DataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(userInfo);
        //     UserApi.Instance.Log(userInfo.UserName, "用户登录");

        //     if (isAutoLogin)
        //     {
        //         CookieManager.Set(CookieManager.UserToken, this, accessToken, expiresAt);
        //     }
        //     else
        //     {
        //         CookieManager.Set(CookieManager.UserToken, this, accessToken);
        //     }

        //     return accessToken;
        // }

        // public void UserLogout()
        // {
        //     CookieManager.Delete(CookieManager.UserToken, this);
        // }
    }
}
