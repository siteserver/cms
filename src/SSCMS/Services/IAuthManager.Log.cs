using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IAuthManager
    {
        Task AddSiteLogAsync(int siteId, string action);

        Task AddSiteLogAsync(int siteId, string action, string summary);

        Task AddSiteLogAsync(int siteId, int channelId, string action, string summary);

        Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary);

        Task AddAdminLogAsync(string action, string summary);

        Task AddAdminLogAsync(string action);

        Task AddUserLogAsync(string action, string summary);

        Task AddUserLogAsync(string action);
    }
}