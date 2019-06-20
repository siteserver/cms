using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IPluginManager
    {
        List<Menu> GetTopMenus(IUrlManager urlManager);

        List<Menu> GetSiteMenus(IUrlManager urlManager, int siteId);

        List<Menu> GetContentMenus(IUrlManager urlManager, List<string> pluginIds, ContentInfo contentInfo);

        List<Permission> GetTopPermissions();

        List<Permission> GetSitePermissions(int siteId);
    }
}
