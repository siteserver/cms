using System.Threading.Tasks;
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
            var admin = await GetAdminAsync();
            if (admin != null)
            {
                var ipAddress = PageUtils.GetIpAddress(_context.HttpContext.Request);
                await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, 0, admin, ipAddress, action, summary);
            }
        }

        public async Task AddSiteLogAsync(int siteId, int channelId, int contentId, string action, string summary)
        {
            var admin = await GetAdminAsync();
            if (admin != null)
            {
                var ipAddress = PageUtils.GetIpAddress(_context.HttpContext.Request);
                await _databaseManager.SiteLogRepository.AddSiteLogAsync(siteId, channelId, contentId, admin, ipAddress, action, summary);
            }
        }

        public async Task AddAdminLogAsync(string action, string summary)
        {
            var admin = await GetAdminAsync();
            if (admin != null)
            {
                var ipAddress = PageUtils.GetIpAddress(_context.HttpContext.Request);
                await _databaseManager.LogRepository.AddAdminLogAsync(admin, ipAddress, action, summary);
            }
        }

        public async Task AddAdminLogAsync(string action)
        {
            var admin = await GetAdminAsync();
            if (admin != null)
            {
                var ipAddress = PageUtils.GetIpAddress(_context.HttpContext.Request);
                await _databaseManager.LogRepository.AddAdminLogAsync(admin, ipAddress, action);
            }
        }

        public async Task AddUserLogAsync(string action, string summary)
        {
            var user = await GetUserAsync();
            if (user != null)
            {
                var ipAddress = PageUtils.GetIpAddress(_context.HttpContext.Request);
                await _databaseManager.LogRepository.AddUserLogAsync(user, ipAddress, action, summary);
            }
        }

        public async Task AddUserLogAsync(string action)
        {
            var user = await GetUserAsync();
            if (user != null)
            {
                var ipAddress = PageUtils.GetIpAddress(_context.HttpContext.Request);
                await _databaseManager.LogRepository.AddUserLogAsync(user, ipAddress, action);
            }
        }
    }
}