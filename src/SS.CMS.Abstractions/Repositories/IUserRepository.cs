using System;
using System.Collections.Generic;
using SS.CMS.Abstractions.Enums;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IUserRepository : IRepository
    {
        int Insert(UserInfo userInfo, string password, string ipAddress, out string errorMessage);

        bool IsPasswordCorrect(string password, out string errorMessage);

        UserInfo Update(UserInfo userInfo, Dictionary<string, object> body, out string errorMessage);

        bool Update(UserInfo userInfo);

        void UpdateLastActivityDateAndCountOfLogin(UserInfo userInfo);

        bool ChangePassword(string userName, string password, out string errorMessage);

        void Check(List<int> idList);

        void Lock(List<int> idList);

        void UnLock(List<int> idList);

        bool IsUserNameExists(string userName);

        bool IsEmailExists(string email);

        bool IsMobileExists(string mobile);

        IList<int> GetIdList(bool isChecked);

        bool CheckPassword(string password, bool isPasswordMd5, string dbPassword, PasswordFormat passwordFormat, string passwordSalt);

        UserInfo Validate(string account, string password, bool isPasswordMd5, out string userName, out string errorMessage);

        // Dictionary<DateTime, int> GetTrackingDictionary(DateTime dateFrom, DateTime dateTo, string xType);

        int GetCount();

        IList<UserInfo> GetUsers(int offset, int limit);

        bool IsExists(int id);

        void Delete(UserInfo userInfo);
    }
}

