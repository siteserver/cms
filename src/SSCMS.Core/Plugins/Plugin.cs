using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins
{
    public class Plugin : IPlugin
    {
        public Plugin(string pluginPath, string folderName)
        {
            FolderName = folderName;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(pluginPath)
                .AddJsonFile(Constants.PluginPackageFileName, optional: false, reloadOnChange: true)
                .AddJsonFile("config.json", optional: true, reloadOnChange: true)
                .Build();

            var assemblyPath = PathUtils.Combine(pluginPath, Main);
            if (FileUtils.IsFileExists(assemblyPath))
            {
                try
                {
                    Assembly = PluginUtils.LoadAssembly(assemblyPath);
                }
                catch
                {
                    // ignored
                }
            }
        }

        public string PluginId => PluginUtils.GetPluginId(this);

        public string FolderName { get; }

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
        public string Main => Configuration[nameof(Main)] ?? $"{FolderName}.dll";
        public List<SiteType> SiteTypes =>
            Configuration.GetSection("extensions:siteTypes").Get<List<SiteType>>();
        public List<Permission> Permissions =>
            Configuration.GetSection("extensions:permissions").Get<List<Permission>>();
        public List<Menu> Menus => Configuration.GetSection("extensions:menus").Get<List<Menu>>();

        // config.json

        public bool Disabled => Configuration.GetValue<bool>(nameof(Disabled));

        public int Taxis => Configuration.GetValue<int>(nameof(Taxis));
    }
}
