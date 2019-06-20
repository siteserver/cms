using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Services
{
    public partial interface IUserManager
    {
        bool IsSuperAdministrator();

        bool IsSiteAdministrator(int siteId);

        Task<IList<int>> GetSiteIdsAsync();

        bool HasAppPermissions(params string[] permissions);

        bool HasSitePermissions();

        bool HasSitePermissions(int siteId);

        bool HasSitePermissions(int siteId, params string[] permissions);

        bool HasChannelPermissions(int siteId, int channelId, params string[] permissions);

        int? GetOnlyAdminId(int siteId, int channelId);
    }
}
