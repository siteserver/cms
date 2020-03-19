using System.Collections.Generic;
using System.Threading.Tasks;

namespace SSCMS
{
    public interface IPermissions
    {
        Task<bool> IsSuperAdminAsync();

        Task<bool> IsSiteAdminAsync();

        Task<string> GetAdminLevelAsync();

        Task<List<int>> GetSiteIdListAsync();

        Task<List<int>> GetChannelIdListAsync(int siteId, params string[] permissions);

        Task<bool> HasSystemPermissionsAsync(params string[] permissions);

        Task<bool> HasSitePermissionsAsync(int siteId, params string[] permissions);

        Task<bool> HasChannelPermissionsAsync(int siteId, int channelId, params string[] permissions);

        Task<List<string>> GetPermissionListAsync();

        Task<bool> HasSitePermissionsAsync(int siteId);

        Task<List<string>> GetSitePermissionsAsync(int siteId);

        Task<bool> HasChannelPermissionsAsync(int siteId, int channelId);

        Task<List<string>> GetChannelPermissionsAsync(int siteId, int channelId);

        Task<List<string>> GetChannelPermissionsAsync(int siteId);
    }
}
