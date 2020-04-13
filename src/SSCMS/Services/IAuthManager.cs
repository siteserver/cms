using System.Threading.Tasks;
using SSCMS.Models;

namespace SSCMS.Services
{
    public partial interface IAuthManager : IPermissions
    {
        Task<User> GetUserAsync();

        Task<Administrator> GetAdminAsync();

        bool IsAdmin { get; }

        int AdminId { get; }

        string AdminName { get; }

        bool IsUser { get; }

        int UserId { get; }

        string UserName { get; }

        bool IsApi { get; }

        string ApiToken { get; }

        Task AddSiteLogAsync(int siteId, string action);

        Task AddSiteLogAsync(int siteId, string action, string summary);

        Task AddSiteLogAsync(int siteId, int channelId, string action, string summary);

        Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary);

        Task AddAdminLogAsync(string action, string summary);

        Task AddAdminLogAsync(string action);
    }
}