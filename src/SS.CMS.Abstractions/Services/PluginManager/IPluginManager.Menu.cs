using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Datory.Utils;

namespace SS.CMS.Abstractions
{
    public partial interface IPluginManager
    {
        Task<string> GetSystemDefaultPageUrlAsync(int siteId);

        Task<string> GetHomeDefaultPageUrlAsync();

        Task<List<Menu>> GetTopMenusAsync();

        Task<List<Menu>> GetSiteMenusAsync(int siteId);

        Task<List<Menu>> GetContentMenusAsync(List<string> pluginIds, Content content);

        Tab GetPluginTab(string pluginId, string prefix, Menu menu);

        Task<List<IPermissions>> GetTopPermissionsAsync();

        Task<List<IPermissions>> GetSitePermissionsAsync(int siteId);
    }
}
