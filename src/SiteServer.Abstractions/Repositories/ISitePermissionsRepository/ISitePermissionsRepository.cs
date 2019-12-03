using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;


namespace SiteServer.Abstractions
{
    public interface ISitePermissionsRepository : IRepository
    {
        Task<int> InsertAsync(SitePermissions permissionsInfo);

        Task<bool> DeleteAsync(string roleName);

        Task<List<string>> GetAppSitePermissionssAsync(IEnumerable<string> roles);

        Task<List<string>> GetSiteSitePermissionssAsync(IEnumerable<string> roles, int siteId);

        Task<List<string>> GetChannelSitePermissionssAsync(IEnumerable<string> roles, int siteId, int channelId);
    }
}