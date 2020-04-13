using System.Collections.Generic;
using System.Threading.Tasks;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Services
{
    public partial interface IOldPluginManager
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
