using System;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Caches;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.CMS.Apis
{
    public class AdminApi : IAdminApi
    {
        private AdminApi() { }

        private static AdminApi _instance;
        public static AdminApi Instance => _instance ?? (_instance = new AdminApi());

        public IAdministratorInfo GetAdminInfoByUserId(int userId)
        {
            return AdminManager.GetAdminInfoByUserId(userId);
        }

        public IAdministratorInfo GetAdminInfoByUserName(string userName)
        {
            return AdminManager.GetAdminInfoByUserName(userName);
        }

        public IAdministratorInfo GetAdminInfoByEmail(string email)
        {
            return AdminManager.GetAdminInfoByEmail(email);
        }

        public IAdministratorInfo GetAdminInfoByMobile(string mobile)
        {
            return AdminManager.GetAdminInfoByMobile(mobile);
        }

        public IAdministratorInfo GetAdminInfoByAccount(string account)
        {
            return AdminManager.GetAdminInfoByAccount(account);
        }

        public List<string> GetUserNameList()
        {
            return DataProvider.Administrator.GetUserNameList().ToList();
        }

        public IPermissions GetPermissions(string userName)
        {
            return new PermissionsImpl(AdminManager.GetAdminInfoByUserName(userName));
        }

        public bool IsUserNameExists(string userName)
        {
            return DataProvider.Administrator.IsUserNameExists(userName);
        }

        public bool IsEmailExists(string email)
        {
            return DataProvider.Administrator.IsEmailExists(email);
        }

        public bool IsMobileExists(string mobile)
        {
            return DataProvider.Administrator.IsMobileExists(mobile);
        }

        public string GetAccessToken(int userId, string userName, TimeSpan expiresAt)
        {
#pragma warning disable CS0612 // '“RequestImpl”已过时
            return RequestImpl.GetAccessToken(userId, userName, expiresAt);
#pragma warning restore CS0612 // '“RequestImpl”已过时
        }

        public IAccessToken ParseAccessToken(string accessToken)
        {
#pragma warning disable CS0612 // '“RequestImpl”已过时
            return RequestImpl.ParseAccessToken(accessToken);
#pragma warning restore CS0612 // '“RequestImpl”已过时
        }
    }
}
