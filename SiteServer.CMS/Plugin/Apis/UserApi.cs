using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Model;
using SiteServer.Plugin.Apis;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin.Apis
{
    public class UserApi : IUserApi
    {
        private UserApi() { }

        public static UserApi Instance { get; } = new UserApi();

        public IUserInfo GetUserInfoByUserId(int userId)
        {
            return BaiRongDataProvider.UserDao.GetUserInfo(userId);
        }

        public IUserInfo GetUserInfoByUserName(string userName)
        {
            return BaiRongDataProvider.UserDao.GetUserInfoByUserName(userName);
        }

        public IUserInfo GetUserInfoByEmail(string email)
        {
            return BaiRongDataProvider.UserDao.GetUserInfoByEmail(email);
        }

        public IUserInfo GetUserInfoByMobile(string mobile)
        {
            return BaiRongDataProvider.UserDao.GetUserInfoByMobile(mobile);
        }

        public string GetMobileByAccount(string account)
        {
            return BaiRongDataProvider.UserDao.GetMobileByAccount(account);
        }

        public bool IsUserNameExists(string userName)
        {
            return BaiRongDataProvider.UserDao.IsUserNameExists(userName);
        }

        public bool IsEmailExists(string email)
        {
            return BaiRongDataProvider.UserDao.IsEmailExists(email);
        }

        public bool IsMobileExists(string mobile)
        {
            return BaiRongDataProvider.UserDao.IsMobileExists(mobile);
        }

        public IUserInfo NewInstance()
        {
            return new UserInfo();
        }

        public bool Insert(IUserInfo userInfo, string password, out string errorMessage)
        {
            return BaiRongDataProvider.UserDao.Insert(userInfo, password, PageUtils.GetIpAddress(), out errorMessage);
        }

        public bool Validate(string account, string password, out string userName, out string errorMessage)
        {
            return BaiRongDataProvider.UserDao.Validate(account, password, out userName, out errorMessage);
        }

        public void UpdateLastActivityDateAndCountOfFailedLogin(string userName)
        {
            BaiRongDataProvider.UserDao.UpdateLastActivityDateAndCountOfFailedLogin(userName);
        }

        public void UpdateLastActivityDateAndCountOfLogin(string userName)
        {
            BaiRongDataProvider.UserDao.UpdateLastActivityDateAndCountOfLogin(userName);
        }

        public bool ChangePassword(string userName, string password, out string errorMessage)
        {
            return BaiRongDataProvider.UserDao.ChangePassword(userName, password, out errorMessage);
        }

        public void Update(IUserInfo userInfo)
        {
            BaiRongDataProvider.UserDao.Update(userInfo);
        }

        public bool IsPasswordCorrect(string password, out string errorMessage)
        {
            return BaiRongDataProvider.UserDao.IsPasswordCorrect(password, out errorMessage);
        }

        public void AddLog(string userName, string action, string summary)
        {
            LogUtils.AddUserLog(userName, action, summary);
        }

        public List<ILogInfo> GetLogs(string userName, int totalNum, string action = "")
        {
            return BaiRongDataProvider.UserLogDao.List(userName, totalNum, action);
        }
    }
}
