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
        private readonly IConfiguration _config;
        private readonly ISettingsManager _settingsManager;
        private List<IPlugin> _plugins;

        public PluginManager(IConfiguration config, ISettingsManager settingsManager)
        {
            _config = config;
            _settingsManager = settingsManager;
            
            DirectoryPath = PathUtils.Combine(settingsManager.ContentRootPath, Constants.PluginsDirectory);
            DirectoryUtils.CreateDirectoryIfNotExists(DirectoryPath);
        }

        public async Task ReloadAsync()
        {
            _plugins = new List<IPlugin>();
            var configurations = new List<IConfiguration>
            {
                _config
            };
            foreach (var folderPath in Directory.GetDirectories(DirectoryPath))
            {
                if (string.IsNullOrEmpty(folderPath)) continue;
                var configPath = PathUtils.Combine(folderPath, Constants.PackageFileName);
                if (!FileUtils.IsFileExists(configPath)) continue;

                var plugin = new Plugin(folderPath, true);
                _plugins.Add(plugin);
                configurations.Add(plugin.Configuration);
            }

            var builder = new ConfigurationBuilder();
            foreach (var configuration in configurations)
            {
                builder.AddConfiguration(configuration);
            }
            Configuration = builder.Build();

            var tables = GetTables();
            foreach (var table in tables.Where(table => !string.IsNullOrEmpty(table.Id) && table.Columns != null && table.Columns.Count > 0))
            {
                try
                {
                    if (!await _settingsManager.Database.IsTableExistsAsync(table.Id))
                    {
                        await _settingsManager.Database.CreateTableAsync(table.Id, table.Columns);
                    }
                    else
                    {
                        await _settingsManager.Database.AlterTableAsync(table.Id,
                            table.Columns);
                    }
                }
                catch
                {
                    // ignored
                }
            }
        }

        public IConfiguration Configuration { get; private set; }

        public string DirectoryPath { get; }

        public List<IPlugin> Plugins => _plugins;

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
