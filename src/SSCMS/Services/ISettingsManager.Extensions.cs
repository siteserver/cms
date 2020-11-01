using System.Collections.Generic;
using SSCMS.Configuration;

namespace SSCMS.Services
{
    public partial interface ISettingsManager
    {
        SiteType GetSiteType(string key);
        List<SiteType> GetSiteTypes();
        List<Permission> GetPermissions();
        List<Menu> GetMenus();
        List<Table> GetTables();
        List<string> GetContentTableNames();
    }
}
