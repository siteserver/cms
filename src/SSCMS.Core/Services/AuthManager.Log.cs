using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using SSCMS.Models;
using SSCMS.Services;
using SSCMS.Utils;

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