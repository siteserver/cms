using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using SSCMS.Plugins;
using SSCMS.Utils;

namespace SSCMS.Core.Plugins
{
    public class Plugin : IPlugin
    {
        public Plugin(string folderPath, string folderName)
        {
            FolderName = folderName;

            Configuration = new ConfigurationBuilder()
                .SetBasePath(folderPath)
                .AddJsonFile(Constants.PackageFileName, optional: false, reloadOnChange: true)
                .Build();

            if (string.IsNullOrEmpty(Main) || !Main.EndsWith(".dll")) return;

            var assemblyPath = Directory.GetFiles(folderPath, Main, SearchOption.AllDirectories).FirstOrDefault();
            if (string.IsNullOrEmpty(assemblyPath)) return;

            try
            {
                Assembly = PluginUtils.LoadAssembly(assemblyPath);
            }
            catch
            {
                // ignored
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

        public bool Disabled => Configuration.GetValue<bool>(nameof(Disabled));

        public int Taxis => Configuration.GetValue<int>(nameof(Taxis));
    }
}
