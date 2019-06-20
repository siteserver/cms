using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
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
        private readonly ISettingsManager _settingsManager;
        private readonly ICacheManager _cacheManager;
        private readonly IPathManager _pathManager;
        private readonly ITableManager _tableManager;
        private readonly IPluginRepository _pluginRepository;
        private readonly ISiteRepository _siteRepository;
        private readonly IChannelRepository _channelRepository;
        private readonly ITableStyleRepository _tableStyleRepository;
        private readonly IErrorLogRepository _errorLogRepository;
        private List<IPluginInstance> _pluginInfoListRunnable;

        public PluginManager(ISettingsManager settingsManager, ICacheManager cacheManager, IPathManager pathManager, ITableManager tableManager, IPluginRepository pluginRepository, ISiteRepository siteRepository, IChannelRepository channelRepository, ITableStyleRepository tableStyleRepository, IErrorLogRepository errorLogRepository)
        {
            _settingsManager = settingsManager;
            _cacheManager = cacheManager;
            _pathManager = pathManager;
            _tableManager = tableManager;
            _pluginRepository = pluginRepository;
            _siteRepository = siteRepository;
            _channelRepository = channelRepository;
            _tableStyleRepository = tableStyleRepository;
            _errorLogRepository = errorLogRepository;
            _pluginInfoListRunnable = PluginInfoListRunnable;
        }

        public IPackageMetadata GetMetadata(string pluginId)
        {
            var dict = GetPluginSortedList();
            IPluginInstance pluginInfo;
            if (dict.TryGetValue(pluginId, out pluginInfo))
            {
                return pluginInfo.Plugin;
            }
            return null;
        }

        public bool IsExists(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return false;

            var dict = GetPluginSortedList();

            return dict.ContainsKey(pluginId);
        }

        public List<IPluginInstance> PluginInfoListRunnable
        {
            get
            {
                var dict = GetPluginSortedList();
                return dict.Values.Where(pluginInfo => pluginInfo.Plugin != null).ToList();
            }
        }

        public List<IPluginInstance> AllPluginInfoList
        {
            get
            {
                var dict = GetPluginSortedList();
                return dict.Values.ToList();
            }
        }

        public List<IPluginInstance> GetEnabledPluginInfoList<T>() where T : PluginBase
        {
            var dict = GetPluginSortedList();
            return
                    dict.Values.Where(
                            pluginInfo =>
                                pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                                pluginInfo.Plugin is T
                        )
                        .ToList();
        }

        public List<IService> Services
        {
            get
            {
                var dict = GetPluginSortedList();

                return dict.Values.Where(
                            pluginInfo =>
                                pluginInfo.Plugin != null && !pluginInfo.IsDisabled
                        ).Select(pluginInfo => pluginInfo.Service).ToList();
            }
        }

        public IPluginInstance GetPluginInfo(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = GetPluginSortedList();

            return dict.TryGetValue(pluginId, out var pluginInfo) ? pluginInfo : null;
        }

        public IPluginInstance GetPluginInfo<T>() where T : PluginBase
        {
            var dict = GetPluginSortedList();
            return dict.Values.Where(instance => instance.Plugin is T).FirstOrDefault(instance => instance.IsRunnable && !instance.IsDisabled);
        }

        public Dictionary<string, string> GetPluginIdAndVersionDict()
        {
            var dict = GetPluginSortedList();

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

        public PluginBase GetPlugin(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = GetPluginSortedList();

            IPluginInstance pluginInfo;
            if (dict.TryGetValue(pluginId, out pluginInfo))
            {
                return pluginInfo.Plugin;
            }
            return null;
        }

        public IPluginInstance GetEnabledPluginInfo<T>(string pluginId) where T : PluginBase
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = GetPluginSortedList();

            IPluginInstance pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return pluginInfo;
            }
            return null;
        }

        public List<IPluginInstance> GetEnabledPluginInfoList<T1, T2>()
        {
            var dict = GetPluginSortedList();

            return dict.Values.Where(
                            pluginInfo =>
                                pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                                (pluginInfo.Plugin is T1 || pluginInfo.Plugin is T2)
                        )
                        .ToList();
        }

        public List<PluginBase> GetEnabledPluginMetadatas<T>() where T : PluginBase
        {
            var dict = GetPluginSortedList();

            return dict.Values.Where(
                        pluginInfo =>
                            pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                            pluginInfo.Plugin is T
                    ).Select(pluginInfo => pluginInfo.Plugin).ToList();
        }

        public IPackageMetadata GetEnabledPluginMetadata<T>(string pluginId) where T : PluginBase
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = GetPluginSortedList();

            IPluginInstance pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return pluginInfo.Plugin;
            }
            return null;
        }

        public T GetEnabledFeature<T>(string pluginId) where T : PluginBase
        {
            if (string.IsNullOrEmpty(pluginId)) return default(T);

            var dict = GetPluginSortedList();

            IPluginInstance pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return (T)pluginInfo.Plugin;
            }
            return default(T);
        }

        public List<T> GetEnabledFeatures<T>() where T : PluginBase
        {
            var dict = GetPluginSortedList();

            var pluginInfos = dict.Values.Where(
                        pluginInfo =>
                            pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                            pluginInfo.Plugin is T
                    )
                    .ToList();
            return pluginInfos.Select(pluginInfo => (T)pluginInfo.Plugin).ToList();
        }

        public IService GetService(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            foreach (var service in Services)
            {
                if (StringUtils.EqualsIgnoreCase(service.PluginId, pluginId))
                {
                    return service;
                }
            }

            return null;
        }

        public void Delete(string pluginId)
        {
            DirectoryUtils.DeleteDirectoryIfExists(_pathManager.GetPluginPath(pluginId));
            ClearCache();
        }

        public void UpdateDisabled(string pluginId, bool isDisabled)
        {
            var pluginInfo = GetPluginInfo(pluginId);
            if (pluginInfo != null)
            {
                pluginInfo.IsDisabled = isDisabled;
                _pluginRepository.UpdateIsDisabled(pluginId, isDisabled);
                ClearCache();
            }
        }

        public void UpdateTaxis(string pluginId, int taxis)
        {
            var pluginInfo = GetPluginInfo(pluginId);
            if (pluginInfo != null)
            {
                pluginInfo.Taxis = taxis;
                _pluginRepository.UpdateTaxis(pluginId, taxis);
                ClearCache();
            }
        }

        public string GetPluginIconUrl(string pluginId)
        {
            foreach (var service in Services)
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

        private readonly object LockObject = new object();
        private const string CacheKey = "SiteServer.CMS.Plugin.PluginCache";

        private SortedList<string, IPluginInstance> Load()
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

                    var pluginInfo = ActivePlugin(directoryName);
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
                _errorLogRepository.AddErrorLog(ex, "载入插件时报错");
            }

            return dict;
        }

        private PluginInstance ActivePlugin(string directoryName)
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

                return ActiveAndAdd(metadata, type);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                _errorLogRepository.AddErrorLog(ex, $"插件加载：{directoryName}");
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

        private PluginInstance ActiveAndAdd(IPackageMetadata metadata, Type type)
        {
            if (metadata == null || type == null) return null;

            if (StringUtils.EqualsIgnoreCase(metadata.Id, "SS.Home")) return null;

            var s = Stopwatch.StartNew();

            //var plugin = (IPlugin)Activator.CreateInstance(type);

            var plugin = (PluginBase)Activator.CreateInstance(type);
            plugin.Initialize(metadata);

            var service = new ServiceImpl(metadata);

            plugin.Startup(service);

            SyncContentTable(service);
            SyncTable(service);

            return new PluginInstance(metadata, service, plugin, s.ElapsedMilliseconds, _pluginRepository);
        }

        public void ClearCache()
        {
            _cacheManager.Remove(CacheKey);
        }

        public SortedList<string, IPluginInstance> GetPluginSortedList()
        {
            var retval = _cacheManager.Get<SortedList<string, IPluginInstance>>(CacheKey);
            if (retval != null) return retval;

            lock (LockObject)
            {
                retval = _cacheManager.Get<SortedList<string, IPluginInstance>>(CacheKey);
                if (retval == null)
                {
                    retval = Load();
                    _cacheManager.InsertHours(CacheKey, retval, 24);
                }
            }

            return retval;
        }
    }
}
