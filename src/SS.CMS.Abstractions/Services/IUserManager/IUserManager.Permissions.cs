using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Services
{
    public partial interface IUserManager
    {
        bool IsAdministrator();
        bool IsSiteAdministrator(int siteId);
        bool IsSuperAdministrator();
        Task<IList<int>> GetSiteIdsAsync();
        bool HasAppPermissions(params string[] permissions);
        Task<bool> HasSitePermissionsAsync();
        bool HasSitePermissions(int siteId);
        bool HasSitePermissions(int siteId, params string[] permissions);
        bool HasChannelPermissions(int siteId, int channelId, params string[] permissions);
        Task<int?> GetOnlyAdminIdAsync(int siteId, int channelId);

        IList<string> GetRoles();
    }
}
