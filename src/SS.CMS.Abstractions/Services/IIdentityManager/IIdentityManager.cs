using System.Threading.Tasks;
using SS.CMS.Abstractions.Models;
using SS.CMS.Abstractions.Repositories;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IIdentityManager
    {
        Task Sync();

        bool IsApiAuthenticated { get; }

        string ApiToken { get; }

        // api end

        // admin start

        string AdminToken { get; }

        bool IsAdminLoggin { get; }

        IPermissions AdminPermissions { get; }

        int AdminId { get; }

        string AdminName { get; }

        AdministratorInfo AdminInfo { get; }

        string AdminLogin(string userName, bool isAutoLogin, IAccessTokenRepository accessTokenRepository);

        void AdminLogout();

        // admin end

        // user start

        bool IsUserLoggin { get; }

        string UserToken { get; }

        IPermissions UserPermissions { get; }

        int UserId { get; }

        string UserName { get; }

        UserInfo UserInfo { get; }

        string UserLogin(string userName, bool isAutoLogin);

        void UserLogout();

        // user end

        // log start

        string IpAddress { get; }

        void AddSiteLog(int siteId, string action, string summary = "");

        void AddChannelLog(int siteId, int channelId, string action, string summary = "");

        void AddContentLog(int siteId, int channelId, int contentId, string action, string summary = "");

        void AddAdminLog(string action, string summary = "");

        void AddUserLog(string action, string summary = "");

        /// <summary>
        /// 获取Access Token字符串。
        /// </summary>
        /// <param name="token">Access Token。</param>
        /// <returns>返回此用户的Access Token。</returns>
        string GetToken(Token token);

        /// <summary>
        /// 解析Access Token字符串。
        /// </summary>
        /// <param name="accessToken">用户Access Token。</param>
        /// <returns>存储于用户Token中的用户名。</returns>
        Token ParseToken(string accessToken);
    }
}
