using System.Collections.Generic;
using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IPluginManager
    {
        List<Menu> GetTopMenus(IUrlManager urlManager);

        List<Menu> GetSiteMenus(IUrlManager urlManager, int siteId);

        List<Menu> GetContentMenus(IUrlManager urlManager, List<string> pluginIds, ContentInfo contentInfo);

        List<KeyValuePair<string, string>> GetTopPermissions();

        List<KeyValuePair<string, string>> GetSitePermissions(int siteId);
    }
}
