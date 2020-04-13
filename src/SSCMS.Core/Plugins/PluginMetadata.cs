using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SSCMS.Plugins;

namespace SSCMS.Core.Plugins
{
    public class PluginMetadata : IPluginMetadata
    {
        public PluginMetadata(string folderName, string pluginPath, Assembly assembly)
        {
            PluginId = folderName;
            Assembly = assembly;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(pluginPath)
                .AddJsonFile("package.json", optional: true, reloadOnChange: true)
                .Build();
        }

        public string PluginId { get; }

        public Assembly Assembly { get; }

        public IConfiguration Configuration { get; }

        public string Name => Configuration[nameof(Name)];
        public string Version => Configuration[nameof(Version)];
        public string Publisher => Configuration[nameof(Publisher)];
        public string Repository => Configuration[nameof(Repository)];
        public string DisplayName => Configuration[nameof(DisplayName)];
        public string Description => Configuration[nameof(Description)];
        public string License => Configuration[nameof(License)];
        public string Icon => Configuration[nameof(Icon)];

        public IEnumerable<string> Categories => Configuration.GetSection(nameof(Categories)).Get<string[]>();
        public IEnumerable<string> Keywords => Configuration.GetSection(nameof(Keywords)).Get<string[]>();
        public string Homepage => Configuration[nameof(Homepage)];
    }
}
