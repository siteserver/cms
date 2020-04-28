using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SSCMS.Core.Plugins;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PluginManager : IPluginManager
    {
        private List<IPlugin> _plugins;
        private readonly IConfiguration _config;

        public PluginManager(IConfiguration config, ISettingsManager settingsManager)
        {
            _config = config;
            DirectoryPath = PathUtils.Combine(settingsManager.ContentRootPath, Constants.PluginsDirectory);
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryPath);
            Reload();   
        }

        public void Reload()
        {
            _plugins = new List<IPlugin>();
            var configurations = new List<IConfiguration>
            {
                _config
            };
            foreach (var folderPath in Directory.GetDirectories(DirectoryPath))
            {
                if (string.IsNullOrEmpty(folderPath)) continue;
                var folderName = Path.GetFileName(folderPath);
                if (string.IsNullOrEmpty(folderName)) continue;
                var configPath = PathUtils.Combine(folderPath, Constants.PackageFileName);
                if (!FileUtils.IsFileExists(configPath)) continue;

                var plugin = new Plugin(folderPath, folderName);
                _plugins.Add(plugin);
                configurations.Add(plugin.Configuration);
            }

            var builder = new ConfigurationBuilder();

            foreach (var configuration in configurations)
            {
                builder.AddConfiguration(configuration);
            }

            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; private set; }

        public string DirectoryPath { get; }

        public IEnumerable<IPlugin> Plugins => _plugins;

        public IPlugin GetPlugin(string pluginId)
        {
            return _plugins.FirstOrDefault(x => PluginUtils.GetPluginId(x) == pluginId);
        }

        public IEnumerable<Assembly> Assemblies => _plugins.Select(x => x.Assembly).Where(x => x != null);

        public async Task SaveConfigAsync(string pluginId, Dictionary<string, object> config)
        {
            var plugin = GetPlugin(pluginId);
            if (plugin != null)
            {
                var configPath = PathUtils.Combine(DirectoryPath, plugin.FolderName, Constants.PluginConfigFileName);
                var configValue = TranslateUtils.JsonSerialize(config);
                await FileUtils.WriteTextAsync(configPath, configValue);
            }
        }

        public IEnumerable<T> GetExtensions<T>(bool useCaching = false) where T : IPluginExtension
        {
            return PluginUtils.GetInstances<T>(Assemblies, useCaching);
        }
    }
}
