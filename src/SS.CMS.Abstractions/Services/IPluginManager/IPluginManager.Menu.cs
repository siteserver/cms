using System.Collections.Generic;
using SS.CMS.Models;

namespace SS.CMS.Services.IPluginManager
{
    public partial interface IPluginManager
    {
        List<Menu> GetTopMenus(IUrlManager.IUrlManager urlManager);

        List<Menu> GetSiteMenus(IUrlManager.IUrlManager urlManager, int siteId);

        List<Menu> GetContentMenus(IUrlManager.IUrlManager urlManager, List<string> pluginIds, ContentInfo contentInfo);

        List<Permission> GetTopPermissions();

        List<Permission> GetSitePermissions(int siteId);
    }
}
