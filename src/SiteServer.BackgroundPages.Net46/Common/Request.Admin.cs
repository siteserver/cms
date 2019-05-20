using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
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
        public string AdminToken
        {
            get
            {
                var accessTokenStr = string.Empty;
                if (TryGetCookie(Constants.AuthKeyAdminCookie, out var cookie))
                {
                    accessTokenStr = cookie;
                }
                else if (TryGetHeader(Constants.AuthKeyAdminHeader, out var headers))
                {
                    var header = headers.SingleOrDefault();
                    accessTokenStr = StringUtils.IsEncrypted(header) ? TranslateUtils.DecryptStringBySecretKey(header) : header;
                }
                else if (IsQueryExists(Constants.AuthKeyAdminQuery))
                {
                    var query = GetQueryString(Constants.AuthKeyAdminQuery);
                    accessTokenStr = StringUtils.IsEncrypted(query) ? TranslateUtils.DecryptStringBySecretKey(query) : query;
                }

                return accessTokenStr;
            }
        }

        public bool IsAdminLoggin { get; private set; }

        private PermissionsImpl _adminPermissionsImpl;

        public PermissionsImpl AdminPermissionsImpl
        {
            get
            {
                if (_adminPermissionsImpl != null) return _adminPermissionsImpl;

                _adminPermissionsImpl = new PermissionsImpl(AdminInfo);

                return _adminPermissionsImpl;
            }
        }

        public IPermissions AdminPermissions => AdminPermissionsImpl;

        public int AdminId => AdminInfo?.Id ?? 0;

        public string AdminName
        {
            get
            {
                if (AdminInfo != null)
                {
                    return AdminInfo.UserName;
                }

                if (UserInfo != null)
                {
                    var groupInfo = UserGroupManager.GetUserGroupInfo(UserInfo.GroupId);
                    if (groupInfo != null)
                    {
                        return groupInfo.AdminName;
                    }
                }

                return string.Empty;
            }
        }

        public IAdministratorInfo AdminInfo => _adminInfo;

        public string AdminLogin(string userName, bool isAutoLogin)
        {
            if (string.IsNullOrEmpty(userName)) return null;
            var adminInfo = AdminManager.GetAdminInfoByUserName(userName);
            if (adminInfo == null || adminInfo.Locked) return null;

            _adminInfo = adminInfo;
            IsAdminLoggin = true;

            var expiresAt = TimeSpan.FromDays(Constants.AccessTokenExpireDays);
            var accessToken = AdminApi.Instance.GetAccessToken(adminInfo.Id, adminInfo.UserName, expiresAt);

            AddAdminLog("管理员登录");

            if (isAutoLogin)
            {
                SetCookie(Constants.AuthKeyAdminCookie, accessToken, expiresAt);
            }
            else
            {
                SetCookie(Constants.AuthKeyAdminCookie, accessToken);
            }

            return accessToken;
        }

        public void AdminLogout()
        {
            RemoveCookie(Constants.AuthKeyAdminCookie);
        }

        // public object AdminRedirectCheck(bool checkInstall = false, bool checkDatabaseVersion = false,
        //     bool checkLogin = false)
        // {
        //     var redirect = false;
        //     var redirectUrl = string.Empty;

        //     if (checkInstall && string.IsNullOrEmpty(AppSettings.ConnectionString))
        //     {
        //         redirect = true;
        //         redirectUrl = PageUtilsEx.GetAdminUrl("Installer/");
        //     }
        //     else if (checkDatabaseVersion && ConfigManager.Instance.Initialized &&
        //              ConfigManager.Instance.DatabaseVersion != SystemManager.ProductVersion)
        //     {
        //         redirect = true;
        //         redirectUrl = PageUtilsEx.GetAdminUrl("pageSyncDatabase.aspx");
        //     }
        //     else if (checkLogin && (!IsAdminLoggin || AdminInfo == null || AdminInfo.Locked))
        //     {
        //         redirect = true;
        //         redirectUrl = PageUtilsEx.GetAdminUrl("pageLogin.cshtml");
        //     }

        //     if (redirect)
        //     {
        //         return new
        //         {
        //             Value = false,
        //             RedirectUrl = redirectUrl
        //         };
        //     }

        //     return null;
        // }

        public object AdminRedirectCheck(bool checkInstall = false, bool checkDatabaseVersion = false,
            bool checkLogin = false)
        {
            var redirect = false;
            var redirectUrl = string.Empty;

            if (checkInstall && string.IsNullOrWhiteSpace(AppSettings.ConnectionString))
            {
                redirect = true;
                redirectUrl = PageUtilsEx.GetAdminUrl("installer/default.aspx");
            }
            else if (checkDatabaseVersion && ConfigManager.Instance.Initialized &&
                     ConfigManager.Instance.DatabaseVersion != SystemManager.ProductVersion)
            {
                redirect = true;
                redirectUrl = AdminPagesUtils.UpdateUrl;
            }
            else if (checkLogin && !IsAdminLoggin)
            {
                redirect = true;
                redirectUrl = AdminPagesUtils.LoginUrl;
            }

            if (redirect)
            {
                return new
                {
                    Value = false,
                    RedirectUrl = redirectUrl
                };
            }

            return null;
        }
    }
}
