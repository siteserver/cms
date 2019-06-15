using System.Collections.Generic;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Repositories
{
    public partial interface IUserRepository
    {
        UserInfo GetUserInfoByUserId(int userId);

        UserInfo GetUserInfoByUserName(string userName);

        UserInfo GetUserInfoByMobile(string mobile);

        UserInfo GetUserInfoByEmail(string email);

        UserInfo GetUserInfoByAccount(string account);

        bool IsIpAddressCached(string ipAddress);

        void CacheIpAddress(string ipAddress);
    }
}
