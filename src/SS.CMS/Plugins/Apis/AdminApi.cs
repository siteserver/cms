using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS;
using SiteServer.CMS.Plugin.Impl;
using SS.CMS.Abstractions;
using SiteServer.CMS.Repositories;
using SS.CMS.Plugins.Impl;

namespace SiteServer.CMS.Plugin.Apis
{
    public class AdminApi
    {
        private AdminApi() { }

        private static AdminApi _instance;
        public static AdminApi Instance => _instance ??= new AdminApi();

        public async Task<Administrator> GetByUserIdAsync(int userId)
        {
            return await DataProvider.AdministratorRepository.GetByUserIdAsync(userId);
        }

        public async Task<Administrator> GetByUserNameAsync(string userName)
        {
            return await DataProvider.AdministratorRepository.GetByUserNameAsync(userName);
        }

        public async Task<Administrator> GetByEmailAsync(string email)
        {
            return await DataProvider.AdministratorRepository.GetByEmailAsync(email);
        }

        public async Task<Administrator> GetByMobileAsync(string mobile)
        {
            return await DataProvider.AdministratorRepository.GetByMobileAsync(mobile);
        }

        public async Task<Administrator> GetByAccountAsync(string account)
        {
            return await DataProvider.AdministratorRepository.GetByAccountAsync(account);
        }

        public async Task<List<string>> GetUserNameListAsync()
        {
            return await DataProvider.AdministratorRepository.GetUserNameListAsync();
        }

        public async Task<PermissionsImpl> GetPermissionsAsync(string userName)
        {
            var adminInfo = await DataProvider.AdministratorRepository.GetByUserNameAsync(userName);
            return new PermissionsImpl(adminInfo);
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            return await DataProvider.AdministratorRepository.IsUserNameExistsAsync(userName);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await DataProvider.AdministratorRepository.IsEmailExistsAsync(email);
        }

        public async Task<bool> IsMobileExistsAsync(string mobile)
        {
            return await DataProvider.AdministratorRepository.IsMobileExistsAsync(mobile);
        }

        public string GetAccessToken(int userId, string userName, TimeSpan expiresAt)
        {
            return AuthenticatedRequest.GetAccessToken(userId, userName, expiresAt);
        }

        public string GetAccessToken(int userId, string userName, DateTime expiresAt)
        {
            return AuthenticatedRequest.GetAccessToken(userId, userName, expiresAt);
        }

        public AccessTokenImpl ParseAccessToken(string accessToken)
        {
            return AuthenticatedRequest.ParseAccessToken(accessToken);
        }
    }
}
