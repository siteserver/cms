using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;
using SiteServer.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SiteServer.BackgroundPages.Common
{
    public partial class Request
    {
        public bool IsUserLoggin { get; }


        private string UserToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (TryGetCookie(Constants.AuthKeyUserCookie, out var cookie))
                {
                    accessTokenStr = cookie;
                }
                else if (TryGetHeader(Constants.AuthKeyUserHeader, out var headers))
                {
                    var header = headers.SingleOrDefault();
                    accessTokenStr = StringUtils.IsEncrypted(header) ? TranslateUtils.DecryptStringBySecretKey(header) : header;
                }
                else if (IsQueryExists(Constants.AuthKeyUserQuery))
                {
                    var query = GetQueryString(Constants.AuthKeyUserQuery);
                    accessTokenStr = StringUtils.IsEncrypted(query) ? TranslateUtils.DecryptStringBySecretKey(query) : query;
                }

                return accessTokenStr;
            }
        }

        private PermissionsImpl _userPermissionsImpl;

        public PermissionsImpl UserPermissionsImpl
        {
            get
            {
                if (_userPermissionsImpl != null) return _userPermissionsImpl;

                if (UserInfo != null)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        _adminInfo = AdminManager.GetAdminInfoByUserName(groupInfo.AdminName);
                    }
                }

                _userPermissionsImpl = new PermissionsImpl(AdminInfo);

                return _userPermissionsImpl;
            }
        }

        public IPermissions UserPermissions => UserPermissionsImpl;

        public int UserId => UserInfo?.Id ?? 0;

        public string UserName => UserInfo?.UserName ?? string.Empty;

        public IUserInfo UserInfo => _userInfo;

        public string UserLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;

            var userInfo = UserManager.GetUserInfoByUserName(userName);
            if (userInfo == null || userInfo.Locked || !userInfo.Checked) return null;

            _userInfo = userInfo;

            var expiresAt = TimeSpan.FromDays(Constants.AccessTokenExpireDays);
            var accessToken = UserApi.Instance.GetAccessToken(UserId, UserName, expiresAt);

            DataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(_userInfo);
            AddUserLog("用户登录");

            if (isAutoLogin)
            {
                SetCookie(Constants.AuthKeyUserCookie, accessToken, expiresAt);
            }
            else
            {
                SetCookie(Constants.AuthKeyUserCookie, accessToken);
            }

            return accessToken;
        }

        public void UserLogout()
        {
            _userInfo = null;
            RemoveCookie(Constants.AuthKeyUserCookie);
        }
    }
}
