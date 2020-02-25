using System.Collections.Generic;
using System.Threading.Tasks;
using Datory;

namespace SS.CMS.Abstractions
{
    public interface ISitePermissionsRepository : IRepository
    {
        Task InsertAsync(SitePermissions permissions);

        Task DeleteAsync(string roleName);

        Task<List<SitePermissions>> GetSystemPermissionsListAsync(string roleName);

        Task<SitePermissions> GetSystemPermissionsAsync(string roleName, int siteId);

        Task<Dictionary<int, List<string>>> GetWebsitePermissionSortedListAsync(IEnumerable<string> roles);

        Task<Dictionary<string, List<string>>> GetChannelPermissionSortedListAsync(IList<string> roles);

        Task<List<string>> GetChannelPermissionListIgnoreChannelIdAsync(IList<string> roles);
    }
}
