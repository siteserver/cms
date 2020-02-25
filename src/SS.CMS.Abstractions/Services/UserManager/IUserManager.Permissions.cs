using System.Collections.Generic;
using System.Threading.Tasks;


namespace SS.CMS.Abstractions
{
    public partial interface IUserManager
    {
        bool IsAdministrator();
        bool IsSiteAdministrator(int siteId);
        bool IsSuperAdministrator();
        Task<IList<int>> GetSiteIdsAsync();
        Task<bool> HasAppPermissionsAsync(params string[] permissions);
        Task<bool> HasSitePermissionsAsync();
        Task<bool> HasSitePermissionsAsync(int siteId);
        Task<bool> HasSitePermissionsAsync(int siteId, params string[] permissions);
        Task<bool> HasChannelPermissionsAsync(int siteId, int channelId, params string[] permissions);
        Task<int?> GetOnlyAdminIdAsync(int siteId, int channelId);

        IList<string> GetRoles();
    }
}
