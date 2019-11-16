using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public async Task<IAdministrator> GetAdminInfoByUserIdAsync(int userId)
        {
            return await AdminManager.GetByUserIdAsync(userId);
        }

        public async Task<IAdministrator> GetAdminInfoByUserNameAsync(string userName)
        {
            return await AdminManager.GetByUserNameAsync(userName);
        }

        public async Task<IAdministrator> GetAdminInfoByEmailAsync(string email)
        {
            return await AdminManager.GetByEmailAsync(email);
        }

        public async Task<IAdministrator> GetAdminInfoByMobileAsync(string mobile)
        {
            return await AdminManager.GetByMobileAsync(mobile);
        }

        public async Task<IAdministrator> GetAdminInfoByAccountAsync(string account)
        {
            return await AdminManager.GetByAccountAsync(account);
        }

        public async Task<IEnumerable<string>> GetUserNameListAsync()
        {
            return await DataProvider.AdministratorDao.GetUserNameListAsync();
        }

        public async Task<IPermissions> GetPermissionsAsync(string userName)
        {
            var adminInfo = await AdminManager.GetByUserNameAsync(userName);
            return new PermissionsImpl(adminInfo);
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            return await DataProvider.AdministratorDao.IsUserNameExistsAsync(userName);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await DataProvider.AdministratorDao.IsEmailExistsAsync(email);
        }

        public async Task<bool> IsMobileExistsAsync(string mobile)
        {
            return await DataProvider.AdministratorDao.IsMobileExistsAsync(mobile);
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
