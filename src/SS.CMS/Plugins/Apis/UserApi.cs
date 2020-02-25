using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS;
using SS.CMS.Abstractions;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.CMS.Repositories;
using SS.CMS.Plugins.Impl;


namespace SiteServer.CMS.Plugin.Apis
{
    public class UserApi
    {
        private UserApi() { }

        private static UserApi _instance;
        public static UserApi Instance => _instance ??= new UserApi();

        public User NewInstance()
        {
            return new User();
        }

        public async Task<User> GetByUserIdAsync(int userId)
        {
            return await DataProvider.UserRepository.GetByUserIdAsync(userId);
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            return await DataProvider.UserRepository.GetByUserNameAsync(userName);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await DataProvider.UserRepository.GetByEmailAsync(email);
        }

        public async Task<User> GetByMobileAsync(string mobile)
        {
            return await DataProvider.UserRepository.GetByMobileAsync(mobile);
        }

        public async Task<User> GetByAccountAsync(string account)
        {
            return await DataProvider.UserRepository.GetByAccountAsync(account);
        }

        public async Task<bool> IsUserNameExistsAsync(string userName)
        {
            return await DataProvider.UserRepository.IsUserNameExistsAsync(userName);
        }

        public async Task<bool> IsEmailExistsAsync(string email)
        {
            return await DataProvider.UserRepository.IsEmailExistsAsync(email);
        }

        public async Task<bool> IsMobileExistsAsync(string mobile)
        {
            return await DataProvider.UserRepository.IsMobileExistsAsync(mobile);
        }

        public async Task<(bool Valid, string ErrorMessage)> InsertAsync(User user, string password)
        {
            var valid = await DataProvider.UserRepository.InsertAsync(user as User, password, PageUtils.GetIpAddress());
            return (valid.UserId > 0, valid.ErrorMessage);
        }

        public async Task<(bool Valid, string UserName, string ErrorMessage)> ValidateAsync(string account, string password)
        {
            var valid = await DataProvider.UserRepository.ValidateAsync(account, password, false);
            return (valid.User != null, valid.UserName, valid.ErrorMessage);
        }

        public async Task<(bool Success, string ErrorMessage)> ChangePasswordAsync(int userId, string password)
        {
            var valid = await DataProvider.UserRepository.ChangePasswordAsync(userId, password);
            return (valid.IsValid, valid.ErrorMessage);
        }

        public async Task UpdateAsync(User userInfo)
        {
            await DataProvider.UserRepository.UpdateAsync(userInfo as User);
        }

        public async Task<(bool Valid, string ErrorMessage)> IsPasswordCorrectAsync(string password)
        {
            return await UserRepository.IsPasswordCorrectAsync(password);
        }

        public async Task AddLogAsync(int userId, string action, string summary)
        {
            await LogUtils.AddUserLogAsync(userId, action, summary);
        }

        public async Task<List<UserLog>> GetLogsAsync(int userId, int totalNum, string action = "")
        {
            return await DataProvider.UserLogRepository.ListAsync(userId, totalNum, action);
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
