using System.Collections.Generic;
using System.Threading.Tasks;

namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager
    {
        Task<List<Menu>> GetTopMenusAsync(IPathManager pathManager);

        Task<List<Menu>> GetSiteMenusAsync(IPathManager pathManager, int siteId);

        Task<List<Menu>> GetContentMenusAsync(IPathManager pathManager, List<string> pluginIds, Content contentInfo);

        Task<List<MenuPermission>> GetTopPermissionsAsync();

        Task<List<MenuPermission>> GetSitePermissionsAsync(int siteId);
    }
}
