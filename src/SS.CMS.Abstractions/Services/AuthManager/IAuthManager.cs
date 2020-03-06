using System;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IAuthManager : IService
    {
        Task<IAuthManager> GetApiAsync();

        Task<IAuthManager> GetUserAsync();

        Task<IAuthManager> GetAdminAsync();

        bool IsApiAuthenticated { get; }

        bool IsUserLoggin { get; }

        bool IsAdminLoggin { get; }

        string ApiToken { get; }

        string AdminToken { get; }

        Task AddSiteLogAsync(int siteId, string action);

        Task AddSiteLogAsync(int siteId, string action, string summary);

        Task AddSiteLogAsync(int siteId, int channelId, string action, string summary);

        Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary);

        Task AddAdminLogAsync(string action, string summary);

        Task AddAdminLogAsync(string action);

        IPermissions UserPermissions { get; }

        IPermissions AdminPermissions { get; }

        int AdminId { get; }

        string AdminName { get; }

        Administrator Administrator { get; }

        Task<string> AdminLoginAsync(string userName, bool isAutoLogin);

        void AdminLogout();

        int UserId { get; }

        string UserName { get; }

        User User { get; }

        Task<string> UserLoginAsync(string userName, bool isAutoLogin);

        void UserLogout();

        string GetAccessToken(int userId, string userName, TimeSpan expiresAt);

        string GetAccessToken(int userId, string userName, DateTime expiresAt);

        //AccessTokenImpl ParseAccessToken(string accessToken);
    }
}