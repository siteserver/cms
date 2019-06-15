using System.Collections.Generic;
using SS.CMS.Abstractions.Models;
using SS.CMS.Data;

namespace SS.CMS.Abstractions.Repositories
{
    public interface ISitePermissionsRepository : IRepository
    {
        int Insert(SitePermissionsInfo permissionsInfo);

        bool Delete(string roleName);

        IList<SitePermissionsInfo> GetSystemPermissionsInfoList(string roleName);

        Dictionary<int, List<string>> GetWebsitePermissionSortedList(IEnumerable<string> roles);

        Dictionary<string, List<string>> GetChannelPermissionSortedList(IList<string> roles);

        List<string> GetChannelPermissionListIgnoreChannelId(IList<string> roles);

        void InsertRoleAndPermissions(string roleName, string creatorUserName, string description, List<string> generalPermissionList, List<SitePermissionsInfo> systemPermissionsInfoList);

        void UpdateSitePermissions(string roleName, List<SitePermissionsInfo> sitePermissionsInfoList);
    }
}