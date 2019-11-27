using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SiteServer.CMS.Core;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class AdminApi : IAdminApi
    {
        private AdminApi() { }

        private static AdminApi _instance;
        public static AdminApi Instance => _instance ??= new AdminApi();

        public async Task<IAdministrator> GetByUserIdAsync(int userId)
        {
            return await DataProvider.AdministratorDao.GetByUserIdAsync(userId);
        }

        public async Task<IAdministrator> GetByUserNameAsync(string userName)
        {
            return await DataProvider.AdministratorDao.GetByUserNameAsync(userName);
        }

        public async Task<IAdministrator> GetByEmailAsync(string email)
        {
            return await DataProvider.AdministratorDao.GetByEmailAsync(email);
        }

        public async Task<IAdministrator> GetByMobileAsync(string mobile)
        {
            return await DataProvider.AdministratorDao.GetByMobileAsync(mobile);
        }

        public async Task<IAdministrator> GetByAccountAsync(string account)
        {
            return await DataProvider.AdministratorDao.GetByAccountAsync(account);
        }

        public async Task<IEnumerable<string>> GetUserNameListAsync()
        {
            return await DataProvider.AdministratorDao.GetUserNameListAsync();
        }

        public async Task<IPermissions> GetPermissionsAsync(string userName)
        {
            var adminInfo = await DataProvider.AdministratorDao.GetByUserNameAsync(userName);
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
