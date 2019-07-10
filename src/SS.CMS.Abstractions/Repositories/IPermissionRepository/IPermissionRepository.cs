using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IPermissionRepository : IRepository
    {
        Task<int> InsertAsync(Models.Permission permissionsInfo);

        Task<bool> DeleteAsync(string roleName);

        Task<List<string>> GetAppPermissionsAsync(IEnumerable<string> roles);

        Task<List<string>> GetSitePermissionsAsync(IEnumerable<string> roles, int siteId);

        Task<List<string>> GetChannelPermissionsAsync(IEnumerable<string> roles, int siteId, int channelId);
    }
}