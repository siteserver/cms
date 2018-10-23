using System;
using System.Collections.Generic;
using SiteServer.CMS.Core;
using SiteServer.CMS.DataCache;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Impl;
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
            return new UserInfo();
        }

        public IUserInfo GetUserInfoByUserId(int userId)
        {
            return UserManager.GetUserInfoByUserId(userId);
        }

        public IUserInfo GetUserInfoByUserName(string userName)
        {
            return UserManager.GetUserInfoByUserName(userName);
        }

        public IUserInfo GetUserInfoByEmail(string email)
        {
            return UserManager.GetUserInfoByEmail(email);
        }

        public IUserInfo GetUserInfoByMobile(string mobile)
        {
            return UserManager.GetUserInfoByMobile(mobile);
        }

        public IUserInfo GetUserInfoByAccount(string account)
        {
            return UserManager.GetUserInfoByAccount(account);
        }

        public bool IsUserNameExists(string userName)
        {
            return DataProvider.UserDao.IsUserNameExists(userName);
        }

        public bool IsEmailExists(string email)
        {
            return DataProvider.UserDao.IsEmailExists(email);
        }

        public bool IsMobileExists(string mobile)
        {
            return DataProvider.UserDao.IsMobileExists(mobile);
        }

        public bool Insert(IUserInfo userInfo, string password, out string errorMessage)
        {
            var userId = DataProvider.UserDao.Insert(userInfo as UserInfo, password, PageUtils.GetIpAddress(), out errorMessage);
            return userId > 0;
        }

        public bool Validate(string account, string password, out string userName, out string errorMessage)
        {
            var userInfo = DataProvider.UserDao.Validate(account, password, false, out userName, out errorMessage);
            return userInfo != null;
        }

        public bool ChangePassword(string userName, string password, out string errorMessage)
        {
            return DataProvider.UserDao.ChangePassword(userName, password, out errorMessage);
        }

        public void Update(IUserInfo userInfo)
        {
            DataProvider.UserDao.Update(userInfo as UserInfo);
        }

        public bool IsPasswordCorrect(string password, out string errorMessage)
        {
            return DataProvider.UserDao.IsPasswordCorrect(password, out errorMessage);
        }

        public void AddLog(string userName, string action, string summary)
        {
            LogUtils.AddUserLog(userName, action, summary);
        }

        public List<ILogInfo> GetLogs(string userName, int totalNum, string action = "")
        {
            return DataProvider.UserLogDao.List(userName, totalNum, action);
        }

        public string GetAccessToken(int userId, string userName, DateTime expiresAt)
        {
            return RequestImpl.GetAccessToken(userId, userName, expiresAt);
        }

        public IAccessToken ParseAccessToken(string accessToken)
        {
            return RequestImpl.ParseAccessToken(accessToken);
        }
    }
}
