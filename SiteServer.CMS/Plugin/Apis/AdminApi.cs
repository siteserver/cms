using System;
using System.Collections.Generic;
using System.Linq;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class AdminApi : IAdminApi
    {
        private AdminApi() { }

        private static AdminApi _instance;
        public static AdminApi Instance => _instance ?? (_instance = new AdminApi());

        public IAdministratorInfo GetAdminInfoByUserId(int userId)
        {
            return AdminManager.GetAdminInfoByUserIdAsync(userId).GetAwaiter().GetResult();
        }

        public IAdministratorInfo GetAdminInfoByUserName(string userName)
        {
            return AdminManager.GetAdminInfoByUserNameAsync(userName).GetAwaiter().GetResult();
        }

        public IAdministratorInfo GetAdminInfoByEmail(string email)
        {
            return AdminManager.GetAdminInfoByEmailAsync(email).GetAwaiter().GetResult();
        }

        public IAdministratorInfo GetAdminInfoByMobile(string mobile)
        {
            return AdminManager.GetAdminInfoByMobileAsync(mobile).GetAwaiter().GetResult();
        }

        public IAdministratorInfo GetAdminInfoByAccount(string account)
        {
            return AdminManager.GetAdminInfoByAccountAsync(account).GetAwaiter().GetResult();
        }

        public List<string> GetUserNameList()
        {
            return DataProvider.AdministratorDao.GetUserNameListAsync().GetAwaiter().GetResult().ToList();
        }

        public IPermissions GetPermissions(string userName)
        {
            var adminInfo = AdminManager.GetAdminInfoByUserNameAsync(userName).GetAwaiter().GetResult();
            return new PermissionsImpl(adminInfo);
        }

        public bool IsUserNameExists(string userName)
        {
            return DataProvider.AdministratorDao.IsUserNameExistsAsync(userName).GetAwaiter().GetResult();
        }

        public bool IsEmailExists(string email)
        {
            return DataProvider.AdministratorDao.IsEmailExistsAsync(email).GetAwaiter().GetResult();
        }

        public bool IsMobileExists(string mobile)
        {
            return DataProvider.AdministratorDao.IsMobileExistsAsync(mobile).GetAwaiter().GetResult();
        }

        public string GetAccessToken(int userId, string userName, TimeSpan expiresAt)
        {
            return AuthenticatedRequest.GetAccessToken(userId, userName, expiresAt);
        }

        public string GetAccessToken(int userId, string userName, DateTime expiresAt)
        {
            return AuthenticatedRequest.GetAccessToken(userId, userName, expiresAt);
        }

        public IAccessToken ParseAccessToken(string accessToken)
        {
            return AuthenticatedRequest.ParseAccessToken(accessToken);
        }
    }
}
