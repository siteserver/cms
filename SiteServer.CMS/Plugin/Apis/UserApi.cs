using System;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Provider;
using SiteServer.Utils;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin.Apis
{
    public class UserApi : IUserApi
    {
        private UserApi() { }

        private static UserApi _instance;
        public static UserApi Instance => _instance ?? (_instance = new UserApi());

        public IUserInfo NewInstance()
        {
            return new User();
        }

        public IUserInfo GetUserInfoByUserId(int userId)
        {
            return UserManager.GetUserByUserIdAsync(userId).GetAwaiter().GetResult();
        }

        public IUserInfo GetUserInfoByUserName(string userName)
        {
            return UserManager.GetUserByUserNameAsync(userName).GetAwaiter().GetResult();
        }

        public IUserInfo GetUserInfoByEmail(string email)
        {
            return UserManager.GetUserByEmailAsync(email).GetAwaiter().GetResult();
        }

        public IUserInfo GetUserInfoByMobile(string mobile)
        {
            return UserManager.GetUserByMobileAsync(mobile).GetAwaiter().GetResult();
        }

        public IUserInfo GetUserInfoByAccount(string account)
        {
            return UserManager.GetUserByAccountAsync(account).GetAwaiter().GetResult();
        }

        public bool IsUserNameExists(string userName)
        {
            return DataProvider.UserDao.IsUserNameExistsAsync(userName).GetAwaiter().GetResult();
        }

        public bool IsEmailExists(string email)
        {
            return DataProvider.UserDao.IsEmailExistsAsync(email).GetAwaiter().GetResult();
        }

        public bool IsMobileExists(string mobile)
        {
            return DataProvider.UserDao.IsMobileExistsAsync(mobile).GetAwaiter().GetResult();
        }

        public bool Insert(IUserInfo user, string password, out string errorMessage)
        {
            var valid = DataProvider.UserDao.InsertAsync(user as User, password, PageUtils.GetIpAddress()).GetAwaiter().GetResult();
            errorMessage = valid.ErrorMessage;
            return valid.UserId > 0;
        }

        public bool Validate(string account, string password, out string userName, out string errorMessage)
        {
            var valid = DataProvider.UserDao.ValidateAsync(account, password, false).GetAwaiter().GetResult();
            userName = valid.UserName;
            errorMessage = valid.ErrorMessage;
            return valid.User != null;
        }

        public bool ChangePassword(string userName, string password, out string errorMessage)
        {
            var valid = DataProvider.UserDao.ChangePasswordAsync(userName, password).GetAwaiter().GetResult();
            errorMessage = valid.ErrorMessage;
            return valid.IsValid;
        }

        public void Update(IUserInfo userInfo)
        {
            DataProvider.UserDao.UpdateAsync(userInfo as User).GetAwaiter().GetResult();
        }

        public bool IsPasswordCorrect(string password, out string errorMessage)
        {
            return UserDao.IsPasswordCorrect(password, out errorMessage);
        }

        public void AddLog(string userName, string action, string summary)
        {
            LogUtils.AddUserLog(userName, action, summary);
        }

        public List<ILogInfo> GetLogs(string userName, int totalNum, string action = "")
        {
            return DataProvider.UserLogDao.List(userName, totalNum, action);
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
