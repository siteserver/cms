using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace SSCMS.Plugins
{
    public interface IPlugin
    {
        public string PluginId { get; }
        public string FolderName { get; }
        [JsonIgnore]
        public Assembly Assembly { get; }
        [JsonIgnore]
        public IConfiguration Configuration { get; }
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
        public int Taxis { get; }
    }
}
