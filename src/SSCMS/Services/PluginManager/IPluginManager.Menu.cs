using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Utils;

namespace SSCMS
{
    public partial interface IPluginManager
    {
        Task<string> GetSystemDefaultPageUrlAsync(int siteId);

        Task<string> GetHomeDefaultPageUrlAsync();

        Task<List<Menu>> GetTopMenusAsync();

        Task<List<Menu>> GetSiteMenusAsync(int siteId);

        Task<List<Menu>> GetContentMenusAsync(List<string> pluginIds, Content content);

        Tab GetPluginTab(string pluginId, string prefix, Menu menu);

        Task<List<PermissionConfig>> GetTopPermissionsAsync();

        Task<List<PermissionConfig>> GetSitePermissionsAsync(int siteId);
    }
}
