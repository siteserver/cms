using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SSCMS.Configuration;

namespace SSCMS.Plugins
{
    public interface IPlugin
    {
        string PluginId { get; }
        [JsonIgnore] string ContentRootPath { get; }
        [JsonIgnore] string WebRootPath { get; }
        [JsonIgnore] Assembly Assembly { get; }
        [JsonIgnore] IConfiguration Configuration { get; }
        string Name { get; }
        string Version { get; }
        string Publisher { get; }
        string Repository { get; }
        string DisplayName { get; }
        string Description { get; }
        string License { get; }
        string Icon { get; }
        IEnumerable<string> Categories { get; }
        IEnumerable<string> Keywords { get; }
        string Homepage { get; }
        string Main { get; }
        bool ApplyToSites { get; }
        bool ApplyToChannels { get; }
        bool Disabled { get; }
        bool AllSites { get; }
        IEnumerable<int> SiteIds { get; }
        IEnumerable<SiteConfig> SiteConfigs { get; }
        IEnumerable<Table> Tables { get; }
        bool Success { get; }
        string ErrorMessage { get; }
        int Taxis { get; }
        List<Menu> GetMenus();
        string GetAssemblyPath();
    }
}
