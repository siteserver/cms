using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IPluginManager
    {
        Task<List<Menu>> GetTopMenusAsync(IUrlManager urlManager);

        Task<List<Menu>> GetSiteMenusAsync(IUrlManager urlManager, int siteId);

        Task<List<Menu>> GetContentMenusAsync(IUrlManager urlManager, List<string> pluginIds, Content contentInfo);

        Task<List<MenuPermission>> GetTopPermissionsAsync();

        Task<List<MenuPermission>> GetSitePermissionsAsync(int siteId);
    }
}
