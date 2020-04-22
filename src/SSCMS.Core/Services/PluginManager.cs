using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using SSCMS.Core.Plugins;
using SSCMS.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public class PluginManager : IPluginManager
    {
        private List<IPlugin> _plugins;

        public PluginManager(ISettingsManager settingsManager)
        {
            DirectoryPath = PathUtils.Combine(settingsManager.ContentRootPath, Constants.PluginsDirectory);
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryPath);
            Reload();   
        }

        public void Reload()
        {
            _plugins = new List<IPlugin>();
            foreach (var folderPath in Directory.GetDirectories(DirectoryPath))
            {
                if (string.IsNullOrEmpty(folderPath)) continue;
                var folderName = Path.GetFileName(folderPath);
                if (string.IsNullOrEmpty(folderName)) continue;
                var configPath = PathUtils.Combine(folderPath, Constants.PluginPackageFileName);
                if (!FileUtils.IsFileExists(configPath)) continue;

                var plugin = new Plugin(folderPath, folderName);
                _plugins.Add(plugin);
            }
        }

        public string DirectoryPath { get; }

        public IEnumerable<IPlugin> Plugins => _plugins.ToArray();

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
