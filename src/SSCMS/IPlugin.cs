using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SSCMS.Dto;

namespace SSCMS
{
    public interface IPlugin
    {
        public string PluginId { get; }
        [JsonIgnore] public string ContentRootPath { get; }
        [JsonIgnore] public string WebRootPath { get; }
        [JsonIgnore] public Assembly Assembly { get; }
        [JsonIgnore] public IConfiguration Configuration { get; }
        public string Name { get; }
        public string Version { get; }
        public string Publisher { get; }
        public string Repository { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public string License { get; }
        public string Icon { get; }
        public IEnumerable<string> Categories { get; }
        public IEnumerable<string> Keywords { get; }
        public string Homepage { get; }
        public string Main { get; }
        public bool Disabled { get; }
        public bool IsAllSites { get; }
        public IEnumerable<int> SiteIds { get; }
        public IEnumerable<SiteConfig> SiteConfigs { get; }
        public bool Success { get; }
        public string ErrorMessage { get; }
        public int Taxis { get; }
    }
}
