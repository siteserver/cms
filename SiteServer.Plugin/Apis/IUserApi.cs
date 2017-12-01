using System.Collections.Generic;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IUserApi
    {
        IUserInfo NewInstance();

        IUserInfo GetUserInfoByUserId(int userId);

        IUserInfo GetUserInfoByUserName(string userName);

        IUserInfo GetUserInfoByEmail(string email);

        IUserInfo GetUserInfoByMobile(string mobile);

        string GetMobileByAccount(string account);

        bool IsUserNameExists(string userName);

        bool IsEmailExists(string email);

        bool IsMobileExists(string mobile);

        bool Insert(IUserInfo userInfo, string password, out string errorMessage);

        bool Validate(string account, string password, out string userName, out string errorMessage);

        void UpdateLastActivityDateAndCountOfFailedLogin(string userName);

        void UpdateLastActivityDateAndCountOfLogin(string userName);

        bool ChangePassword(string userName, string password, out string errorMessage);

        void Update(IUserInfo userInfo);

        bool IsPasswordCorrect(string password, out string errorMessage);

        void AddLog(string userName, string action, string summary);

        List<ILogInfo> GetLogs(string userName, int totalNum, string action = "");
    }
}
