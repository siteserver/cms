using System.Threading.Tasks;

namespace SSCMS.Core.Services
{
    public partial class AuthManager
    {
        public async Task AddSiteLogAsync(int siteId, string action)
        {
            await AddSiteLogAsync(siteId, 0, 0, action, string.Empty);
        }

        public async Task AddSiteLogAsync(int siteId, string action, string summary)
        {
            await AddSiteLogAsync(siteId, 0, 0, action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, string action, string summary)
        {
            await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, 0, await GetAdminAsync(), action, summary);
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary)
        {
            await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, contentId, await GetAdminAsync(), action, summary);
        }

        public async Task AddAdminLogAsync(string action, string summary)
        {
            await _databaseManager.LogRepository.AddAdminLogAsync(await GetAdminAsync(), action, summary);
        }

        public async Task AddAdminLogAsync(string action)
        {
            await _databaseManager.LogRepository.AddAdminLogAsync(await GetAdminAsync(), action);
        }

        public async Task AddUserLogAsync(string action, string summary)
        {
            await _databaseManager.LogRepository.AddUserLogAsync(await GetUserAsync(), action, summary);
        }

        public async Task AddUserLogAsync(string action)
        {
            await _databaseManager.LogRepository.AddUserLogAsync(await GetUserAsync(), action);
        }
    }
}