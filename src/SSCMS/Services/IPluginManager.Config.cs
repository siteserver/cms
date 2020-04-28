using System.Collections.Generic;

namespace SSCMS.Services
{
    public partial interface IPluginManager
    {
        SiteType GetSiteType(string key);
        List<SiteType> GetSiteTypes();
        List<Permission> GetPermissions();
        List<Menu> GetMenus();
    }
}
