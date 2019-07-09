using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using SS.CMS.Core.Plugin;
using SS.CMS.Repositories;
using SS.CMS.Services;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    /// <summary>
    /// The entry for managing SiteServer plugins
    /// </summary>
    public partial class PluginManager : IPluginManager
    {
        private readonly IDistributedCache _cache;
        private readonly string _cacheKey;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly ITableManager _tableManager;
        private readonly IPluginRepository _pluginRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public PluginManager(IDistributedCache cache, ISettingsManager settingsManager, IPathManager pathManager, ITableManager tableManager, IPluginRepository pluginRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, ITableStyleRepository tableStyleRepository, IErrorLogRepository errorLogRepository)
        {
            _cache = cache;
            _cacheKey = _cache.GetKey(nameof(PluginManager));
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _tableManager = tableManager;
            _pluginRepository = pluginRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _tableStyleRepository = tableStyleRepository;
            _errorLogRepository = errorLogRepository;
        }

        public async Task<IPackageMetadata> GetMetadataAsync(string pluginId)
        {
            var dict = await GetPluginSortedListAsync();
            IPluginInstance pluginInfo;
            if (dict.TryGetValue(pluginId, out pluginInfo))
            {
                return pluginInfo.Plugin;
            }
            return null;
        }

        public async Task<bool> IsExistsAsync(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return false;

            var dict = await GetPluginSortedListAsync();

            return dict.ContainsKey(pluginId);
        }

        public async Task<List<IPluginInstance>> GetPluginInfoListRunnableAsync()
        {
            var dict = await GetPluginSortedListAsync();
            return dict.Values.Where(pluginInfo => pluginInfo.Plugin != null).ToList();
        }

        public async Task<List<IPluginInstance>> GetAllPluginInfoListAsync()
        {
            var dict = await GetPluginSortedListAsync();
            return dict.Values.ToList();
        }

        public async Task<List<IPluginInstance>> GetEnabledPluginInfoListAsync<T>() where T : PluginBase
        {
            var dict = await GetPluginSortedListAsync();
            return
                    dict.Values.Where(
                            pluginInfo =>
                                pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                                pluginInfo.Plugin is T
                        )
                        .ToList();
        }

        public async Task<List<IService>> GetServicesAsync()
        {
            var dict = await GetPluginSortedListAsync();

            return dict.Values.Where(
                        pluginInfo =>
                            pluginInfo.Plugin != null && !pluginInfo.IsDisabled
                    ).Select(pluginInfo => pluginInfo.Service).ToList();
        }

        public async Task<IPluginInstance> GetPluginInfoAsync(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = await GetPluginSortedListAsync();

            return dict.TryGetValue(pluginId, out var pluginInfo) ? pluginInfo : null;
        }

        public async Task<IPluginInstance> GetPluginInfoAsync<T>() where T : PluginBase
        {
            var dict = await GetPluginSortedListAsync();
            return dict.Values.Where(instance => instance.Plugin is T).FirstOrDefault(instance => instance.IsRunnable && !instance.IsDisabled);
        }

        public async Task<Dictionary<string, string>> GetPluginIdAndVersionDictAsync()
        {
            var dict = await GetPluginSortedListAsync();

            var retval = new Dictionary<string, string>();

            foreach (var pluginId in dict.Keys)
            {
                var pluginInfo = dict[pluginId];
                if (pluginInfo.Metadata != null)
                {
                    retval[pluginId] = pluginInfo.Metadata.Version;
                }
                else
                {
                    retval[pluginId] = string.Empty;
                }
            }

            return retval;
        }

        public List<string> PackagesIdAndVersionList
        {
            get
            {
                var packagesPath = _pathManager.GetPackagesPath();
                DirectoryUtils.CreateDirectoryIfNotExists(packagesPath);
                return DirectoryUtils.GetDirectoryNames(packagesPath).ToList();
            }
        }

        public async Task<PluginBase> GetPluginAsync(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = await GetPluginSortedListAsync();

            IPluginInstance pluginInfo;
            if (dict.TryGetValue(pluginId, out pluginInfo))
            {
                return pluginInfo.Plugin;
            }
            return null;
        }

        public async Task<IPluginInstance> GetEnabledPluginInfoAsync<T>(string pluginId) where T : PluginBase
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = await GetPluginSortedListAsync();

            IPluginInstance pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return pluginInfo;
            }
            return null;
        }

        public async Task<List<IPluginInstance>> GetEnabledPluginInfoListAsync<T1, T2>()
        {
            var dict = await GetPluginSortedListAsync();

            return dict.Values.Where(
                            pluginInfo =>
                                pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                                (pluginInfo.Plugin is T1 || pluginInfo.Plugin is T2)
                        )
                        .ToList();
        }

        public async Task<List<PluginBase>> GetEnabledPluginMetadatasAsync<T>() where T : PluginBase
        {
            var dict = await GetPluginSortedListAsync();

            return dict.Values.Where(
                        pluginInfo =>
                            pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                            pluginInfo.Plugin is T
                    ).Select(pluginInfo => pluginInfo.Plugin).ToList();
        }

        public async Task<IPackageMetadata> GetEnabledPluginMetadataAsync<T>(string pluginId) where T : PluginBase
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = await GetPluginSortedListAsync();

            IPluginInstance pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return pluginInfo.Plugin;
            }
            return null;
        }

        public async Task<T> GetEnabledFeatureAsync<T>(string pluginId) where T : PluginBase
        {
            if (string.IsNullOrEmpty(pluginId)) return default(T);

            var dict = await GetPluginSortedListAsync();

            IPluginInstance pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return (T)pluginInfo.Plugin;
            }
            return default(T);
        }

        public async Task<List<T>> GetEnabledFeaturesAsync<T>() where T : PluginBase
        {
            var dict = await GetPluginSortedListAsync();

            var pluginInfos = dict.Values.Where(
                        pluginInfo =>
                            pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                            pluginInfo.Plugin is T
                    )
                    .ToList();
            return pluginInfos.Select(pluginInfo => (T)pluginInfo.Plugin).ToList();
        }

        public async Task<IService> GetServiceAsync(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            foreach (var service in await GetServicesAsync())
            {
                if (StringUtils.EqualsIgnoreCase(service.PluginId, pluginId))
                {
                    return service;
                }
            }

            return null;
        }

        public async Task DeleteAsync(string pluginId)
        {
            DirectoryUtils.DeleteDirectoryIfExists(_pathManager.GetPluginPath(pluginId));
            await _cache.RemoveAsync(_cacheKey);
        }

        public async Task UpdateDisabledAsync(string pluginId, bool isDisabled)
        {
            var pluginInfo = await GetPluginInfoAsync(pluginId);
            if (pluginInfo != null)
            {
                pluginInfo.IsDisabled = isDisabled;
                await _pluginRepository.UpdateIsDisabledAsync(pluginId, isDisabled);
                await _cache.RemoveAsync(_cacheKey);
            }
        }

        public async Task UpdateTaxisAsync(string pluginId, int taxis)
        {
            var pluginInfo = await GetPluginInfoAsync(pluginId);
            if (pluginInfo != null)
            {
                pluginInfo.Taxis = taxis;
                await _pluginRepository.UpdateTaxisAsync(pluginId, taxis);
                await _cache.RemoveAsync(_cacheKey);
            }
        }

        public async Task<string> GetPluginIconUrlAsync(string pluginId)
        {
            foreach (var service in await GetServicesAsync())
            {
                if (service.PluginId == pluginId)
                {
                    return GetPluginIconUrl(service);
                }
            }
            return string.Empty;
        }

        public string GetPluginIconUrl(IService service)
        {
            var url = string.Empty;
            if (service.Metadata.IconUrl != null)
            {
                url = service.Metadata.IconUrl.ToString();
            }
            return url;
        }

        // cache



        private async Task<SortedList<string, IPluginInstance>> LoadAsync()
        {
            var dict = new SortedList<string, IPluginInstance>();

            Thread.Sleep(2000);

            try
            {
                var pluginsPath = _pathManager.PluginsPath;
                if (!Directory.Exists(pluginsPath))
                {
                    return dict;
                }

                var directoryNames = DirectoryUtils.GetDirectoryNames(pluginsPath);
                foreach (var directoryName in directoryNames)
                {
                    if (StringUtils.StartsWith(directoryName, ".") || StringUtils.EqualsIgnoreCase(directoryName, "packages")) continue;

                    var pluginInfo = await ActivePluginAsync(directoryName);
                    if (pluginInfo != null)
                    {
                        dict[directoryName] = pluginInfo;
                    }
                }

#if DEBUG
                RunDebugger();
#endif
            }
            catch (Exception ex)
            {
                await _errorLogRepository.AddErrorLogAsync(ex, "载入插件时报错");
            }

            return dict;
        }

        private async Task<PluginInstance> ActivePluginAsync(string directoryName)
        {
            IPackageMetadata metadata = null;
            string errorMessage;

            try
            {
                metadata = GetPackageMetadataFromPluginDirectory(directoryName, out errorMessage);

                var dllDirectoryPath = _pathManager.GetPluginDllDirectoryPath(directoryName);
                if (string.IsNullOrEmpty(dllDirectoryPath))
                {
                    throw new Exception($"插件可执行文件 {directoryName}.dll 不存在");
                }

                //foreach (var filePath in DirectoryUtils.GetFilePaths(DirectoryUtils.GetDirectoryPath(metadata.ExecuteFilePath)))
                //{

                //    if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".dll")) continue;
                //    var fileName = PathUtils.GetFileName(filePath);
                //    if (StringUtils.EqualsIgnoreCase(fileName, PathUtils.GetFileName(metadata.ExecuteFilePath))) continue;
                //    if (FileUtils.IsFileExists(PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "Bin", fileName))) continue;
                //    Assembly.Load(File.ReadAllBytes(filePath));
                //}
                //var assembly = Assembly.Load(File.ReadAllBytes(metadata.ExecuteFilePath));

                //metadata.GetDependencyGroups()

                CopyDllsToBin(metadata.Id, dllDirectoryPath);

                //var assembly = Assembly.Load(File.ReadAllBytes(PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "Bin", PathUtils.GetFileName(metadata.ExecuteFilePath))));
                var assembly = Assembly.Load(metadata.Id);  // load the dll from bin directory

                var type = assembly.GetExportedTypes().FirstOrDefault(exportedType => typeof(PluginBase).IsAssignableFrom(exportedType));

                //var type = assembly.GetTypes().First(o => o.IsClass && !o.IsAbstract && o.IsSubclassOf(typeof(PluginBase)));

                return await ActiveAndAddAsync(metadata, type);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                await _errorLogRepository.AddErrorLogAsync(ex, $"插件加载：{directoryName}");
            }

            return new PluginInstance(directoryName, metadata, errorMessage);
        }

        private void CopyDllsToBin(string pluginId, string pluginDllDirectoryPath)
        {
            foreach (var filePath in DirectoryUtils.GetFilePaths(pluginDllDirectoryPath))
            {
                if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".dll")) continue;

                var fileName = PathUtils.GetFileName(filePath);
                var binFilePath = PathUtils.Combine(_settingsManager.ContentRootPath, fileName);

                if (!FileUtils.IsFileExists(binFilePath))
                {
                    FileUtils.MoveFile(filePath, binFilePath, false);
                }
                else if (StringUtils.EqualsIgnoreCase(fileName, pluginId + ".dll"))
                {
                    if (FileUtils.ComputeHash(filePath) != FileUtils.ComputeHash(binFilePath))
                    {
                        FileUtils.MoveFile(filePath, binFilePath, true);
                    }
                }
            }
        }

        private async Task<PluginInstance> ActiveAndAddAsync(IPackageMetadata metadata, Type type)
        {
            if (metadata == null || type == null) return null;

            if (StringUtils.EqualsIgnoreCase(metadata.Id, "SS.Home")) return null;

            var s = Stopwatch.StartNew();

            //var plugin = (IPlugin)Activator.CreateInstance(type);

            var plugin = (PluginBase)Activator.CreateInstance(type);
            plugin.Initialize(metadata);

            var service = new ServiceImpl(metadata);

            plugin.Startup(service);

            await SyncContentTableAsync(service);
            await SyncTableAsync(service);

            return new PluginInstance(metadata, service, plugin, s.ElapsedMilliseconds, _pluginRepository);
        }

        public async Task<SortedList<string, IPluginInstance>> GetPluginSortedListAsync()
        {
            return await _cache.GetOrCreateAsync(_cacheKey, async options =>
            {
                return await LoadAsync();
            });
        }
    }
}
