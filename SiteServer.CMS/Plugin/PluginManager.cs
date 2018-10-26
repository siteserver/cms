using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using SiteServer.Utils;
using SiteServer.CMS.Core;
using SiteServer.CMS.Packaging;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.CMS.Plugin.Impl;
using SiteServer.Plugin;

namespace SiteServer.CMS.Plugin
{
    /// <summary>
    /// The entry for managing SiteServer plugins
    /// </summary>
    public static class PluginManager
    {
        private static class PluginManagerCache
        {
            private static readonly object LockObject = new object();
            private const string CacheKey = "SiteServer.CMS.Plugin.PluginCache";

            private static SortedList<string, PluginInstance> Load()
            {
                var dict = new SortedList<string, PluginInstance>();

                Thread.Sleep(2000);

                try
                {
                    var pluginsPath = PathUtils.PluginsPath;
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
                    PluginDebugger.Instance.Run();
#endif
                }
                catch (Exception ex)
                {
                    LogUtils.AddErrorLog(ex, "载入插件时报错");
                }

                return dict;
            }

            private static PluginInstance ActivePlugin(string directoryName)
            {
                PackageMetadata metadata = null;
                string errorMessage;

                try
                {
                    metadata = PackageUtils.GetPackageMetadataFromPlugins(directoryName, out errorMessage);

                    var dllDirectoryPath = PathUtils.GetPluginDllDirectoryPath(directoryName);
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
                    LogUtils.AddErrorLog(ex, $"插件加载：{directoryName}");
                }

                return new PluginInstance(directoryName, metadata, errorMessage);
            }

            private static void CopyDllsToBin(string pluginId, string pluginDllDirectoryPath)
            {
                foreach (var filePath in DirectoryUtils.GetFilePaths(pluginDllDirectoryPath))
                {
                    if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".dll")) continue;

                    var fileName = PathUtils.GetFileName(filePath);
                    var binFilePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "Bin", fileName);

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

            private static PluginInstance ActiveAndAdd(PackageMetadata metadata, Type type)
            {
                if (metadata == null || type == null) return null;

                if (StringUtils.EqualsIgnoreCase(metadata.Id, "SS.Home")) return null;

                var s = Stopwatch.StartNew();

                //var plugin = (IPlugin)Activator.CreateInstance(type);

                var plugin = (PluginBase)Activator.CreateInstance(type);
                plugin.Initialize(metadata);

                var service = new ServiceImpl(metadata);

                plugin.Startup(service);

                PluginContentTableManager.SyncContentTable(service);
                PluginDatabaseTableManager.SyncTable(service);

                return new PluginInstance(metadata, service, plugin, s.ElapsedMilliseconds);
            }

            public static void Clear()
            {
                CacheUtils.Remove(CacheKey);
            }

            public static SortedList<string, PluginInstance> GetPluginSortedList()
            {
                var retval = CacheUtils.Get<SortedList<string, PluginInstance>>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = CacheUtils.Get<SortedList<string, PluginInstance>>(CacheKey);
                    if (retval == null)
                    {
                        retval = Load();
                        CacheUtils.InsertHours(CacheKey, retval, 24);
                    }
                }

                return retval;
            }
        }

        private static List<PluginInstance> _pluginInfoListRunnable;

        public static void LoadPlugins(string applicationPhysicalPath)
        {
            WebConfigUtils.Load(applicationPhysicalPath);
            _pluginInfoListRunnable = PluginInfoListRunnable;

            Context.Initialize(new EnvironmentImpl(WebConfigUtils.DatabaseType, WebConfigUtils.ConnectionString, WebConfigUtils.AdminDirectory, WebConfigUtils.PhysicalApplicationPath), new ApiCollectionImpl
            {
                AdminApi = AdminApi.Instance,
                ConfigApi = ConfigApi.Instance,
                ContentApi = ContentApi.Instance,
                DatabaseApi = DataProvider.DatabaseApi,
                ChannelApi = ChannelApi.Instance,
                ParseApi = ParseApi.Instance,
                PluginApi = PluginApi.Instance,
                SiteApi = SiteApi.Instance,
                UserApi = UserApi.Instance,
                UtilsApi = UtilsApi.Instance
            });
        }

        public static void ClearCache()
        {
            PluginManagerCache.Clear();
        }

        public static IMetadata GetMetadata(string pluginId)
        {
            var dict = PluginManagerCache.GetPluginSortedList();
            PluginInstance pluginInfo;
            if (dict.TryGetValue(pluginId, out pluginInfo))
            {
                return pluginInfo.Plugin;
            }
            return null;
        }

        public static bool IsExists(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return false;

            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.ContainsKey(pluginId);
        }

        public static List<PluginInstance> PluginInfoListRunnable
        {
            get
            {
                var dict = PluginManagerCache.GetPluginSortedList();
                return dict.Values.Where(pluginInfo => pluginInfo.Plugin != null).ToList();
            }
        }

        public static List<PluginInstance> AllPluginInfoList
        {
            get
            {
                var dict = PluginManagerCache.GetPluginSortedList();
                return dict.Values.ToList();
            }
        }

        public static List<PluginInstance> GetEnabledPluginInfoList<T>() where T : PluginBase
        {
            var dict = PluginManagerCache.GetPluginSortedList();
            return
                    dict.Values.Where(
                            pluginInfo =>
                                pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                                pluginInfo.Plugin is T
                        )
                        .ToList();
        }

        public static List<ServiceImpl> Services
        {
            get
            {
                var dict = PluginManagerCache.GetPluginSortedList();

                return dict.Values.Where(
                            pluginInfo =>
                                pluginInfo.Plugin != null && !pluginInfo.IsDisabled
                        ).Select(pluginInfo => pluginInfo.Service).ToList();
            }
        }

        public static PluginInstance GetPluginInfo(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.TryGetValue(pluginId, out var pluginInfo) ? pluginInfo : null;
        }

        public static PluginInstance GetPluginInfo<T>() where T : PluginBase
        {
            var dict = PluginManagerCache.GetPluginSortedList();
            return dict.Values.Where(instance => instance.Plugin is T).FirstOrDefault(instance => instance.IsRunnable && !instance.IsDisabled);
        }

        public static Dictionary<string, string> GetPluginIdAndVersionDict()
        {
            var dict = PluginManagerCache.GetPluginSortedList();

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

        public static List<string> PackagesIdAndVersionList
        {
            get
            {
                var packagesPath = PathUtils.GetPackagesPath();
                DirectoryUtils.CreateDirectoryIfNotExists(packagesPath);
                return DirectoryUtils.GetDirectoryNames(packagesPath).ToList();
            }
        }

        public static PluginBase GetPlugin(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginInstance pluginInfo;
            if (dict.TryGetValue(pluginId, out pluginInfo))
            {
                return pluginInfo.Plugin;
            }
            return null;
        }

        public static PluginInstance GetEnabledPluginInfo<T>(string pluginId) where T : PluginBase
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginInstance pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return pluginInfo;
            }
            return null;
        }

        public static List<PluginInstance> GetEnabledPluginInfoList<T1, T2>()
        {
            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.Values.Where(
                            pluginInfo =>
                                pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                                (pluginInfo.Plugin is T1 || pluginInfo.Plugin is T2)
                        )
                        .ToList();
        }

        public static List<PluginBase> GetEnabledPluginMetadatas<T>() where T : PluginBase
        {
            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.Values.Where(
                        pluginInfo =>
                            pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                            pluginInfo.Plugin is T
                    ).Select(pluginInfo => pluginInfo.Plugin).ToList();
        }

        public static IMetadata GetEnabledPluginMetadata<T>(string pluginId) where T : PluginBase
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginInstance pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return pluginInfo.Plugin;
            }
            return null;
        }

        public static T GetEnabledFeature<T>(string pluginId) where T : PluginBase
        {
            if (string.IsNullOrEmpty(pluginId)) return default(T);

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginInstance pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return (T)pluginInfo.Plugin;
            }
            return default(T);
        }

        public static List<T> GetEnabledFeatures<T>() where T : PluginBase
        {
            var dict = PluginManagerCache.GetPluginSortedList();

            var pluginInfos = dict.Values.Where(
                        pluginInfo =>
                            pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                            pluginInfo.Plugin is T
                    )
                    .ToList();
            return pluginInfos.Select(pluginInfo => (T)pluginInfo.Plugin).ToList();
        }

        public static ServiceImpl GetService(string pluginId)
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



        //public static List<ContentModelInfo> GetAllContentModels(SiteInfo siteInfo)
        //{
        //    var cacheName = nameof(GetAllContentModels) + siteInfo.Id;
        //    var contentModels = GetCache<List<ContentModelInfo>>(cacheName);
        //    if (contentModels != null) return contentModels;

        //    contentModels = new List<ContentModelInfo>();

        //    foreach (var pluginInfo in GetEnabledPluginInfoLists<IContentModel>())
        //    {
        //        var model = pluginInfo.Plugin as IContentModel;

        //        if (model == null) continue;

        //        var tableName = siteInfo.AuxiliaryTableForContent;
        //        var tableType = EAuxiliaryTableType.BackgroundContent;
        //        if (model.ContentTableColumns != null && model.ContentTableColumns.Count > 0)
        //        {
        //            tableName = pluginInfo.Id;
        //            tableType = EAuxiliaryTableType.Custom;
        //        }

        //        contentModels.Add(new ContentModelInfo(
        //            pluginInfo.Id,
        //            pluginInfo.Id,
        //            $"插件：{pluginInfo.Metadata.DisplayName}",
        //            tableName,
        //            tableType,
        //            PageUtils.GetPluginDirectoryUrl(pluginInfo.Id, pluginInfo.Metadata.Icon))
        //        );
        //    }

        //    SetCache(cacheName, contentModels);

        //    return contentModels;
        //}



        //public static List<ContentModelInfo> GetAllContentModels(SiteInfo siteInfo)
        //{
        //    var cacheName = nameof(GetAllContentModels) + siteInfo.Id;
        //    var contentModels = GetCache<List<ContentModelInfo>>(cacheName);
        //    if (contentModels != null) return contentModels;

        //    contentModels = new List<ContentModelInfo>();

        //    foreach (var pluginInfo in GetEnabledPluginInfoLists<IContentModel>())
        //    {
        //        var model = pluginInfo.Plugin as IContentModel;

        //        if (model == null) continue;

        //        var links = new List<PluginContentLink>();
        //        if (model.ContentLinks != null)
        //        {
        //            links.AddRange(model.ContentLinks.Select(link => new PluginContentLink
        //            {
        //                Text = link.Text,
        //                Href = PageUtils.GetPluginDirectoryUrl(pluginInfo.Id, link.Href),
        //                Target = link.Target
        //            }));
        //        }
        //        var tableName = siteInfo.AuxiliaryTableForContent;
        //        var tableType = EAuxiliaryTableType.BackgroundContent;
        //        if (model.IsCustomContentTable && model.CustomContentTableColumns != null && model.CustomContentTableColumns.Count > 0)
        //        {
        //            tableName = pluginInfo.Id;
        //            tableType = EAuxiliaryTableType.Custom;
        //        }

        //        contentModels.Add(new ContentModelInfo(
        //            pluginInfo.Id,
        //            pluginInfo.Id,
        //            $"插件：{pluginInfo.Metadata.DisplayName}",
        //            tableName,
        //            tableType,
        //            PageUtils.GetPluginDirectoryUrl(pluginInfo.Id, pluginInfo.Metadata.Icon),
        //            links)
        //        );
        //    }

        //    SetCache(cacheName, contentModels);

        //    return contentModels;
        //}



        //public static Dictionary<string, Func<PluginRenderContext, string>> GetRenders()
        //{
        //    var renders = new Dictionary<string, Func<PluginRenderContext, string>>();

        //    var pluginInfoList = GetEnabledPluginInfoList<IRender>();
        //    if (pluginInfoList != null && pluginInfoList.Count > 0)
        //    {
        //        foreach (var pluginInfo in pluginInfoList)
        //        {
        //            var plugin = pluginInfo.Plugin as IRender;
        //            if (plugin?.Render != null)
        //            {
        //                renders.Add(pluginInfo.Metadata.Id, plugin.Render);
        //            }
        //            //if (!(pluginInfo.Plugin is IRender plugin)) continue;

        //            //if (plugin.Render != null)
        //            //{
        //            //    renders.Add(pluginInfo.Metadata.Id, plugin.Render);
        //            //}
        //        }
        //    }

        //    return renders;
        //}

        //public static List<Action<object, FileSystemEventArgs>> GetFileSystemChangedActions()
        //{
        //    var actions = new List<Action<object, FileSystemEventArgs>>();

        //    var plugins = GetEnabledFeatures<IFileSystem>();
        //    if (plugins != null && plugins.Count > 0)
        //    {
        //        foreach (var plugin in plugins)
        //        {
        //            if (plugin.FileSystemChanged != null)
        //            {
        //                actions.Add(plugin.FileSystemChanged);
        //            }
        //        }
        //    }

        //    return actions;
        //}

        

        //public static bool Install(string pluginId, string version, out string errorMessage)
        //{
        //    errorMessage = string.Empty;
        //    if (string.IsNullOrEmpty(pluginId)) return false;

        //    try
        //    {
        //        if (IsExists(pluginId))
        //        {
        //            errorMessage = $"插件 {pluginId} 已存在";
        //            return false;
        //        }
        //        var directoryPath = PathUtils.GetPluginPath(pluginId);
        //        DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

        //        var zipFilePath = PathUtility.GetTemporaryFilesPath(pluginId + ".zip");
        //        FileUtils.DeleteFileIfExists(zipFilePath);

        //        var downloadUrl = $"http://download.siteserver.cn/plugins/{pluginId}/{version}/{pluginId}.zip";
        //        WebClientUtils.SaveRemoteFileToLocal(downloadUrl, zipFilePath);
                
        //        ZipUtils.UnpackFiles(zipFilePath, directoryPath);
        //        FileUtils.DeleteFileIfExists(zipFilePath);

        //        string dllDirectoryPath;
        //        var metadata = GetPluginMetadata(pluginId, out dllDirectoryPath, out errorMessage);
        //        if (metadata == null)
        //        {
        //            return false;
        //        }

        //        //SaveMetadataToJson(metadata);
        //    }
        //    catch (Exception ex)
        //    {
        //        errorMessage = ex.Message;
        //        return false;
        //    }

        //    return true;
        //}

        public static void Delete(string pluginId)
        {
            DirectoryUtils.DeleteDirectoryIfExists(PathUtils.GetPluginPath(pluginId));
            ClearCache();
        }

        public static void UpdateDisabled(string pluginId, bool isDisabled)
        {
            var pluginInfo = GetPluginInfo(pluginId);
            if (pluginInfo != null)
            {
                pluginInfo.IsDisabled = isDisabled;
                DataProvider.PluginDao.UpdateIsDisabled(pluginId, isDisabled);
                ClearCache();
            }
        }

        public static void UpdateTaxis(string pluginId, int taxis)
        {
            var pluginInfo = GetPluginInfo(pluginId);
            if (pluginInfo != null)
            {
                pluginInfo.Taxis = taxis;
                DataProvider.PluginDao.UpdateTaxis(pluginId, taxis);
                ClearCache();
            }
        }

        //public static PluginMetadata UpdateDatabase(string pluginId, string databaseType, string connectionString)
        //{
        //    var metadata = GetMetadata(pluginId);
        //    if (metadata != null)
        //    {
        //        if (WebConfigUtils.IsProtectData && !string.IsNullOrEmpty(databaseType))
        //        {
        //            databaseType = TranslateUtils.EncryptStringBySecretKey(databaseType);
        //        }
        //        if (WebConfigUtils.IsProtectData && !string.IsNullOrEmpty(connectionString))
        //        {
        //            connectionString = TranslateUtils.EncryptStringBySecretKey(connectionString);
        //        }
        //        metadata.DatabaseType = databaseType;
        //        metadata.ConnectionString = connectionString;
        //        SaveMetadataToJson(metadata);
        //    }
        //    return metadata;
        //}

        /// <summary>
        /// Parse plugin metadata in giving directories
        /// </summary>
        /// <returns></returns>
        //internal static PluginMetadata GetMetadataFromJson(string directoryPath)
        //{
        //    var configPath = Path.Combine(directoryPath, PluginConfigName);
        //    if (!File.Exists(configPath))
        //    {
        //        return null;
        //    }

        //    PluginMetadata metadata;
        //    try
        //    {
        //        metadata = JsonConvert.DeserializeObject<PluginMetadata>(File.ReadAllText(configPath));
        //        metadata.DirectoryPath = directoryPath;
        //    }
        //    catch
        //    {
        //        return null;
        //    }

        //    if (string.IsNullOrEmpty(metadata.Id))
        //    {
        //        return null;
        //    }

        //    return metadata;
        //}

        //private static PackageMetadata GetPluginMetadata(string directoryName, out string dllDirectoryPath, out string errorMessage)
        //{
        //    dllDirectoryPath = string.Empty;
        //    var nuspecPath = PathUtils.GetPluginNuspecPath(directoryName);
        //    if (!File.Exists(nuspecPath))
        //    {
        //        errorMessage = $"插件配置文件 {directoryName}.nuspec 不存在";
        //        return null;
        //    }
        //    dllDirectoryPath = PathUtils.GetPluginDllDirectoryPath(directoryName);
        //    if (string.IsNullOrEmpty(dllDirectoryPath))
        //    {
        //        errorMessage = $"插件可执行文件 {directoryName}.dll 不存在";
        //        return null;
        //    }

        //    PackageMetadata metadata;
        //    try
        //    {
        //        metadata = PackageUtils.GetPackageMetadata(nuspecPath);
        //    }
        //    catch(Exception ex)
        //    {
        //        errorMessage = ex.Message;
        //        return null;
        //    }

        //    if (string.IsNullOrEmpty(metadata.Id))
        //    {
        //        errorMessage = "插件配置文件不正确";
        //        return null;
        //    }

        //    errorMessage = string.Empty;
        //    return metadata;
        //}

        public static string GetPluginIconUrl(string pluginId)
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

        public static string GetPluginIconUrl(ServiceImpl service)
        {
            var url = string.Empty;
            if (service.Metadata.IconUrl != null)
            {
                url = service.Metadata.IconUrl.ToString();
            }
            return url;
        }
    }
}
