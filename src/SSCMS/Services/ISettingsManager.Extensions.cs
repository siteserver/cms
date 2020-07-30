using System.Collections.Generic;
using System.Threading.Tasks;
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
    }
}
