using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using SSCMS.Configuration;
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

        public PluginManager(IConfiguration config, ISettingsManager settingsManager)
        {
            _config = config;
            _settingsManager = settingsManager;

            DirectoryPath = _settingsManager.Containerized
                ? PathUtils.Combine(settingsManager.WebRootPath, DirectoryUtils.SiteFiles.DirectoryName,
                    Constants.PluginsDirectory)
                : PathUtils.Combine(settingsManager.ContentRootPath, Constants.PluginsDirectory);
        }

        public void Load()
        {
            var plugins = new List<Plugin>();
            Plugins = new List<IPlugin>();
            var configurations = new List<IConfiguration>
            {
                _config
            };
            if (!_settingsManager.IsDisablePlugins && DirectoryUtils.IsDirectoryExists(DirectoryPath))
            {
                foreach (var folderPath in DirectoryUtils.GetDirectoryPaths(DirectoryPath))
                {
                    if (string.IsNullOrEmpty(folderPath)) continue;
                    var configPath = PathUtils.Combine(folderPath, Constants.PackageFileName);
                    if (!FileUtils.IsFileExists(configPath)) continue;

                    var plugin = new Plugin(folderPath, true);
                    if (!StringUtils.IsStrictName(plugin.Publisher) || !StringUtils.IsStrictName(plugin.Name)) continue;
                    if (PathUtils.GetFileName(folderPath) != plugin.PluginId) continue;

                    plugins.Add(plugin);
                }

                AssemblyLoadContext.Default.Resolving += (context, name) =>
                {
                    var filePath = PathUtils.Combine(_settingsManager.ContentRootPath, $"{name.Name}.dll");
                    if (FileUtils.IsFileExists(filePath))
                    {
                        return context.LoadFromAssemblyPath(filePath);
                    }
                    var files = Directory.GetFiles(DirectoryPath, $"{name.Name}.dll", SearchOption.AllDirectories);
                    if (files != null && files.Length > 0)
                    {
                        return context.LoadFromAssemblyPath(files[0]);
                    }
                    return null;
                };

                foreach (var plugin in plugins.OrderBy(x => x.Taxis == 0 ? int.MaxValue : x.Taxis))
                {
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
            }

            var builder = new ConfigurationBuilder();
            foreach (var configuration in configurations)
            {
                builder.AddConfiguration(configuration);
            }
            _settingsManager.Configuration = builder.Build();
        }

        //public IPlugin Current
        //{
        //    get
        //    {
        //        var assembly = Assembly.GetCallingAssembly();
        //        return assembly == null ? null : NetCorePlugins.FirstOrDefault(x => x.Assembly.FullName == assembly.FullName);
        //    }
        //}

        public string DirectoryPath { get; }

        public List<IPlugin> Plugins { get; private set; }

        public List<IPlugin> EnabledPlugins => Plugins.Where(x => x.Success && !x.Disabled).ToList();

        public List<IPlugin> NetCorePlugins => EnabledPlugins.Where(x => x.Assembly != null).ToList();

        public List<IPlugin> GetPlugins(int siteId)
        {
            return EnabledPlugins.Where(plugin => IsEnabled(plugin, siteId)).ToList();
        }

        public List<IPlugin> GetPlugins(int siteId, int channelId)
        {
            var plugins = GetPlugins(siteId);
            return plugins.Where(plugin => IsEnabled(plugin, siteId, channelId)).ToList();
        }

        private static bool IsEnabled(IPlugin plugin, int siteId)
        {
            if (plugin == null || plugin.Disabled) return false;
            return plugin.ApplyToSites && (plugin.AllSites || ListUtils.Contains(plugin.SiteIds, siteId));
        }

        private static bool IsEnabled(IPlugin plugin, int siteId, int channelId)
        {
            if (!IsEnabled(plugin, siteId)) return false;
            var siteConfig = plugin.SiteConfigs?.FirstOrDefault(x => x.SiteId == siteId);
            if (siteConfig == null) return false;
            return siteConfig.AllChannels || ListUtils.Contains(siteConfig.ChannelIds, channelId);
        }

        public bool IsEnabled(string pluginId, int siteId)
        {
            var plugin = GetPlugin(pluginId);
            return IsEnabled(plugin, siteId);
        }

        public bool IsEnabled(string pluginId, int siteId, int channelId)
        {
            var plugin = GetPlugin(pluginId);
            return IsEnabled(plugin, siteId, channelId);
        }

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
