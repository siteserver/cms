using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS.Services
{
    public partial interface IAuthManager
    {
        Task<bool> IsSuperAdminAsync();

        Task<bool> IsSiteAdminAsync();

        Task<bool> IsSiteAdminAsync(int siteId);

        Task<string> GetAdminLevelAsync();

        Task<List<int>> GetSiteIdsAsync();

        Task<List<int>> GetChannelPermissionsChannelIdsAsync(int siteId);

        Task<List<int>> GetContentPermissionsChannelIdsAsync(int siteId);

        Task<List<int>> GetVisibleChannelIdsAsync(List<int> channelIdsWithPermissions);

        Task<List<int>> GetChannelPermissionsChannelIdsAsync(int siteId, params string[] permissions);

        Task<List<int>> GetContentPermissionsChannelIdsAsync(int siteId, params string[] permissions);

        Task<bool> HasAppPermissionsAsync(params string[] permissions);

        Task<List<string>> GetAppPermissionsAsync();

        Task<bool> HasSitePermissionsAsync(int siteId);

        Task<bool> HasSitePermissionsAsync(int siteId, params string[] permissions);

        Task<List<string>> GetSitePermissionsAsync(int siteId);

        Task<bool> HasChannelPermissionsAsync(int siteId, int channelId, params string[] permissions);

        Task<bool> HasChannelPermissionsAsync(int siteId, int channelId);

        Task<List<string>> GetChannelPermissionsAsync(int siteId, int channelId);

        Task<List<string>> GetChannelPermissionsAsync(int siteId);

        Task<bool> HasContentPermissionsAsync(int siteId, int channelId, params string[] permissions);

        Task<bool> HasContentPermissionsAsync(int siteId, int channelId);

        Task<List<string>> GetContentPermissionsAsync(int siteId, int channelId);

        Task<List<string>> GetContentPermissionsAsync(int siteId);
    }
}
