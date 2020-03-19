using System;
using System.Threading.Tasks;

namespace SSCMS
{
    public interface IAuthManager : IPermissions
    {
        string GetApiToken();

        string GetUserToken();

        string GetAdminToken();

        Task<User> GetUserAsync();

        Task<Administrator> GetAdminAsync();

        Task<bool> IsApiAuthenticatedAsync();

        Task<bool> IsUserAuthenticatedAsync();

        Task<bool> IsAdminAuthenticatedAsync();

        Task<int> GetAdminIdAsync();

        Task<string> GetAdminNameAsync();

        Task<int> GetUserIdAsync();

        Task<string> GetUserNameAsync();

        Task AddSiteLogAsync(int siteId, string action);

        Task AddSiteLogAsync(int siteId, string action, string summary);

        Task AddSiteLogAsync(int siteId, int channelId, string action, string summary);

        Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary);

        Task AddAdminLogAsync(string action, string summary);

        Task AddAdminLogAsync(string action);

        Task<string> AdminLoginAsync(string userName, bool isAutoLogin);

        void AdminLogout();

        Task<string> UserLoginAsync(string userName, bool isAutoLogin);

        void UserLogout();

        string GetAccessToken(int userId, string userName, TimeSpan expiresAt);

        string GetAccessToken(int userId, string userName, DateTime expiresAt);
    }
}