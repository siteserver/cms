using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SSCMS.Core.Plugins;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class PluginManager : IPluginManager
    {
        private readonly IConfiguration _config;
        private readonly ISettingsManager _settingsManager;
        private readonly string _directoryPath;

        public PluginManager(IConfiguration config, ISettingsManager settingsManager)
        {
            _config = config;
            _settingsManager = settingsManager;

            _directoryPath = PathUtils.Combine(settingsManager.ContentRootPath, Constants.PluginsDirectory);
            DirectoryUtils.CreateDirectoryIfNotExists(_directoryPath);
        }

        public void Load()
        {
            Plugins = new List<IPlugin>();
            var configurations = new List<IConfiguration>
            {
                _config
            };
            foreach (var folderPath in Directory.GetDirectories(_directoryPath))
            {
                if (string.IsNullOrEmpty(folderPath)) continue;
                var configPath = PathUtils.Combine(folderPath, Constants.PackageFileName);
                if (!FileUtils.IsFileExists(configPath)) continue;

                var plugin = new Plugin(folderPath, true);
                if (!StringUtils.IsStrictName(plugin.Publisher) || !StringUtils.IsStrictName(plugin.Name)) continue;
                if (Path.GetFileName(folderPath) != plugin.PluginId) continue;

                Plugins.Add(plugin);
                if (!plugin.Disabled)
                {
                    var (success, errorMessage) = plugin.LoadAssembly();
                    plugin.Success = success;
                    plugin.ErrorMessage = errorMessage;
                    if (success)
                    {
                        configurations.Add(plugin.Configuration);
                    }
                }
            }

            var builder = new ConfigurationBuilder();
            foreach (var configuration in configurations)
            {
                builder.AddConfiguration(configuration);
            }
            Configuration = builder.Build();
        }

        public IPlugin Current
        {
            get
            {
                var assembly = Assembly.GetCallingAssembly();
                return assembly == null ? null : NetCorePlugins.FirstOrDefault(x => x.Assembly.FullName == assembly.FullName);
            }
        }

        public IConfiguration Configuration { get; private set; }

        public List<IPlugin> Plugins { get; private set; }

        public List<IPlugin> EnabledPlugins => Plugins.Where(x => x.Success && !x.Disabled).ToList();

        public List<IPlugin> NetCorePlugins => EnabledPlugins.Where(x => x.Assembly != null).ToList();

        public IPlugin GetPlugin(string pluginId)
        {
            return Plugins.FirstOrDefault(x => x.PluginId == pluginId);
        }

        public IEnumerable<T> GetExtensions<T>(bool useCaching = true) where T : IPluginExtension
        {
            var provider = _settingsManager.BuildServiceProvider();
            return PluginUtils.GetInstances<T>(NetCorePlugins, provider, useCaching);
        }

        public async Task<Dictionary<string, object>> GetConfigAsync(string pluginId)
        {
            var json = string.Empty;
            var plugin = GetPlugin(pluginId);
            if (plugin != null)
            {
                var configPath = PathUtils.Combine(plugin.ContentRootPath, Constants.PluginConfigFileName);
                if (FileUtils.IsFileExists(configPath))
                {
                    json = await FileUtils.ReadTextAsync(configPath);
                }
            }

            return TranslateUtils.ToDictionaryIgnoreCase(json);
        }

        public async Task SaveConfigAsync(string pluginId, Dictionary<string, object> config)
        {
            var plugin = GetPlugin(pluginId);
            if (plugin != null)
            {
                var configPath = PathUtils.Combine(plugin.ContentRootPath, Constants.PluginConfigFileName);
                var configValue = TranslateUtils.JsonSerialize(config);
                await FileUtils.WriteTextAsync(configPath, configValue);
            }
        }
    }
}
