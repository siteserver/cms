using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IPermissionRepository : IRepository
    {
        int Insert(PermissionInfo permissionsInfo);

        bool Delete(string roleName);

        List<string> GetAppPermissions(IEnumerable<string> roles);

        List<string> GetSitePermissions(IEnumerable<string> roles, int siteId);

        List<string> GetChannelPermissions(IEnumerable<string> roles, int siteId, int channelId);
    }
}