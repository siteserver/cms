using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using SiteServer.Utils;
using SiteServer.Utils.IO;
using SiteServer.Utils.Model.Enumerations;
using NuGet;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.CMS.Plugin.Core;
using SiteServer.CMS.Plugin.Model;
using SiteServer.Plugin;
using SiteServer.Plugin.Features;
using IFileSystem = SiteServer.Plugin.Features.IFileSystem;

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
            private static readonly FileWatcherClass FileWatcher;
            
            private static FileSystemWatcher _watcher;

            static PluginManagerCache()
            {
                FileWatcher = new FileWatcherClass(FileWatcherClass.Plugin);
                FileWatcher.OnFileChange += FileWatcher_OnFileChange;
            }

            private static void FileWatcher_OnFileChange(object sender, EventArgs e)
            {
                CacheUtils.Remove(CacheKey);
            }

            private static SortedList<string, PluginInfo> Load()
            {
                Environment = new PluginEnvironment(EDatabaseTypeUtils.GetValue(WebConfigUtils.DatabaseType), WebConfigUtils.ConnectionString, WebConfigUtils.AdminDirectory, WebConfigUtils.PhysicalApplicationPath);
                var dict = new SortedList<string, PluginInfo>();

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

                    _watcher = new FileSystemWatcher
                    {
                        Path = pluginsPath,
                        NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.FileName | NotifyFilters.DirectoryName,
                        IncludeSubdirectories = true
                    };
                    _watcher.Created += Watcher_EventHandler;
                    _watcher.Changed += Watcher_EventHandler;
                    _watcher.Deleted += Watcher_EventHandlerDelete;
                    _watcher.Renamed += Watcher_EventHandler;
                    _watcher.EnableRaisingEvents = true;
                }
                catch (Exception ex)
                {
                    LogUtils.AddSystemErrorLog(ex, "载入插件时报错");
                }

                return dict;
            }

            private static PluginInfo ActivePlugin(string directoryName)
            {
                string errorMessage;

                try
                {
                    string dllDirectoryPath;
                    var metadata = GetPluginMetadata(directoryName, out dllDirectoryPath, out errorMessage);
                    if (metadata != null)
                    {
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
                        var assembly = Assembly.Load(metadata.Id);

                        var type = assembly.GetTypes().First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(IPlugin)));
                        var plugin = (IPlugin)Activator.CreateInstance(type);

                        return ActiveAndAdd(metadata, plugin);
                    }
                }
                catch (Exception ex)
                {
                    errorMessage = ex.Message;
                    LogUtils.AddSystemErrorLog(ex, $"插件加载：{directoryName}");
                }

                return new PluginInfo(directoryName, errorMessage);
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

            private static PluginInfo ActiveAndAdd(IMetadata metadata, IPlugin plugin)
            {
                if (metadata == null || plugin == null) return null;

                var s = Stopwatch.StartNew();

                var context = new PluginContext
                {
                    Environment = Environment,
                    Metadata = metadata,
                    AdminApi = new AdminApi(metadata),
                    ConfigApi = new ConfigApi(metadata),
                    ContentApi = ContentApi.Instance,
                    DataApi = DataProvider.DataApi,
                    FilesApi = new FilesApi(metadata),
                    NodeApi = NodeApi.Instance,
                    ParseApi = ParseApi.Instance,
                    PaymentApi = PaymentApi.Instance,
                    PublishmentSystemApi = PublishmentSystemApi.Instance,
                    SmsApi = SmsApi.Instance,
                    UserApi = UserApi.Instance
                };
                plugin.PluginActive?.Invoke(context);

                var contentModel = plugin as IContentModel;
                PluginTableUtils.SyncContentModel(contentModel, metadata);

                var table = plugin as ITable;
                PluginTableUtils.SyncTable(table, metadata);

                return new PluginInfo(context, plugin, s.ElapsedMilliseconds);
            }

            private static void Watcher_EventHandler(object sender, FileSystemEventArgs e)
            {
                var fullPath = e.FullPath.ToLower();
                if (!fullPath.EndsWith(".nuspec") && !fullPath.EndsWith(".dll")) return;

                try
                {
                    _watcher.EnableRaisingEvents = false;
                    OnConfigOrDllChanged(e.FullPath);
                }
                finally
                {
                    _watcher.EnableRaisingEvents = true;
                }
            }

            private static void Watcher_EventHandlerDelete(object sender, FileSystemEventArgs e)
            {
                if (!PathUtils.IsDirectoryPath(e.FullPath)) return;

                try
                {
                    _watcher.EnableRaisingEvents = false;
                    OnDirectoryDeleted(e.FullPath);
                }
                finally
                {
                    _watcher.EnableRaisingEvents = true;
                }
            }

            private static void OnConfigOrDllChanged(string fullPath)
            {
                var directoryName = PathUtils.GetDirectoryName(fullPath);

                if (string.IsNullOrEmpty(directoryName)) return;

                var plugin = PluginInfoListRunnable.FirstOrDefault(pluginInfo => StringUtils.EqualsIgnoreCase(directoryName, pluginInfo.Id));
                if (plugin != null)
                {
                    Clear();
                }
            }

            private static void OnDirectoryDeleted(string fullPath)
            {
                var directoryName = PathUtils.GetDirectoryName(fullPath);

                var isPlugin = false;
                foreach (var pluginInfo in PluginInfoListRunnable)
                {
                    if (!StringUtils.EqualsIgnoreCase(directoryName, pluginInfo.Id)) continue;
                    isPlugin = true;
                    break;
                }

                if (isPlugin)
                {
                    Clear();
                }
            }

            public static void Clear()
            {
                CacheUtils.Remove(CacheKey);
                FileWatcher.UpdateCacheFile();
            }

            public static SortedList<string, PluginInfo> GetPluginSortedList()
            {
                var retval = CacheUtils.Get<SortedList<string, PluginInfo>>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = CacheUtils.Get<SortedList<string, PluginInfo>>(CacheKey);
                    if (retval == null)
                    {
                        retval = Load();
                        CacheUtils.InsertHours(CacheKey, retval, 24);
                    }
                }

                return retval;
            }
        }

        public static PluginEnvironment Environment { get; private set; }

        public static void ClearCache()
        {
            PluginManagerCache.Clear();
        }

        public static IMetadata GetMetadata(string pluginId)
        {
            var dict = PluginManagerCache.GetPluginSortedList();
            PluginInfo pluginInfo;
            if (dict.TryGetValue(pluginId, out pluginInfo))
            {
                return pluginInfo.Metadata;
            }
            return null;
        }

        public static bool IsExists(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return false;

            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.ContainsKey(pluginId);
        }

        public static List<PluginInfo> PluginInfoListRunnable
        {
            get
            {
                var dict = PluginManagerCache.GetPluginSortedList();
                return dict.Values.Where(pluginInfo => pluginInfo?.Context != null && pluginInfo.Metadata != null && pluginInfo.Plugin != null).ToList();
            }
        }

        public static List<PluginInfo> PluginInfoListNotRunnable
        {
            get
            {
                var dict = PluginManagerCache.GetPluginSortedList();
                return dict.Values.Where(pluginInfo => pluginInfo?.Context == null || pluginInfo.Metadata == null || pluginInfo.Plugin == null).ToList();
            }
        }

        public static List<PluginInfo> AllPluginInfoList
        {
            get
            {
                var dict = PluginManagerCache.GetPluginSortedList();
                return dict.Values.ToList();
            }
        }

        public static List<PluginInfo> GetEnabledPluginInfoList<T>() where T : IPlugin
        {
            var dict = PluginManagerCache.GetPluginSortedList();
            return
                    dict.Values.Where(
                            pluginInfo =>
                                pluginInfo?.Metadata != null && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                                pluginInfo.Plugin is T
                        )
                        .ToList();
        }

        public static PluginInfo GetPluginInfo(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginInfo pluginInfo;
            if (dict.TryGetValue(pluginId, out pluginInfo))
            {
                return pluginInfo;
            }
            return null;
        }

        public static PluginInfo GetEnabledPluginInfo<T>(string pluginId) where T : IPlugin
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginInfo pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo?.Metadata != null && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return pluginInfo;
            }
            return null;
        }

        public static List<PluginInfo> GetEnabledPluginInfoList<T1, T2>()
        {
            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.Values.Where(
                            pluginInfo =>
                                pluginInfo?.Metadata != null && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                                (pluginInfo.Plugin is T1 || pluginInfo.Plugin is T2)
                        )
                        .ToList();
        }

        public static List<IMetadata> GetEnabledPluginMetadatas<T>() where T : IPlugin
        {
            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.Values.Where(
                        pluginInfo =>
                            pluginInfo?.Metadata != null && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                            pluginInfo.Plugin is T
                    ).Select(pluginInfo => pluginInfo.Metadata).ToList();
        }

        public static IMetadata GetEnabledPluginMetadata<T>(string pluginId) where T : IPlugin
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginInfo pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo?.Metadata != null && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return pluginInfo.Metadata;
            }
            return null;
        }

        public static T GetEnabledFeature<T>(string pluginId) where T : IPlugin
        {
            if (string.IsNullOrEmpty(pluginId)) return default(T);

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginInfo pluginInfo;
            var isGet = dict.TryGetValue(pluginId, out pluginInfo);
            if (isGet && pluginInfo?.Metadata != null && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                pluginInfo.Plugin is T)
            {
                return (T)pluginInfo.Plugin;
            }
            return default(T);
        }

        public static List<T> GetEnabledFeatures<T>() where T : IPlugin
        {
            var dict = PluginManagerCache.GetPluginSortedList();

            var pluginInfos = dict.Values.Where(
                        pluginInfo =>
                            pluginInfo?.Metadata != null && pluginInfo.Plugin != null && !pluginInfo.IsDisabled &&
                            pluginInfo.Plugin is T
                    )
                    .ToList();
            return pluginInfos.Select(pluginInfo => (T)pluginInfo.Plugin).ToList();
        }

        public static List<PermissionConfig> GetTopPermissions()
        {
            var permissions = new List<PermissionConfig>();
            foreach (var pluginInfo in GetEnabledPluginInfoList<IMenu>())
            {
                permissions.Add(new PermissionConfig(pluginInfo.Id,
                        $"系统管理 -> {pluginInfo.Metadata.Title}（插件）"));
            }

            return permissions;
        }

        public static List<PermissionConfig> GetSitePermissions(int siteId)
        {
            var pluginInfoList = GetEnabledPluginInfoList<IMenu>();
            var permissions = new List<PermissionConfig>();

            foreach (var pluginInfo in pluginInfoList)
            {
                permissions.Add(new PermissionConfig(pluginInfo.Id, $"{pluginInfo.Metadata.Title}（插件）"));
            }

            return permissions;
        }

        public static Dictionary<string, Menu> GetTopMenus()
        {
            var menus = new Dictionary<string, Menu>();

            var pluginInfoList = GetEnabledPluginInfoList<IMenu>();
            if (pluginInfoList != null && pluginInfoList.Count > 0)
            {
                foreach (var pluginInfo in pluginInfoList)
                {
                    var feature = pluginInfo.Plugin as IMenu;
                    if (feature == null) continue;

                    try
                    {
                        var metadataMenu = feature.PluginMenu;
                        var pluginMenu = GetMenu(pluginInfo.Id, 0, metadataMenu, 0);
                        menus.Add(pluginInfo.Id, pluginMenu);
                    }
                    catch (Exception ex)
                    {
                        LogUtils.AddPluginErrorLog(pluginInfo.Id, ex);
                    }
                }
            }

            return menus;
        }

        public static Dictionary<string, Menu> GetSiteMenus(int siteId)
        {
            var pluginInfoList = GetEnabledPluginInfoList<IMenu>();
            if (pluginInfoList == null || pluginInfoList.Count == 0) return null;

            var menus = new Dictionary<string, Menu>();

            foreach (var pluginInfo in pluginInfoList)
            {
                var feature = pluginInfo.Plugin as IMenu;

                Menu metadataMenu = null;
                try
                {
                    metadataMenu = feature?.SiteMenu?.Invoke(siteId);
                }
                catch (Exception ex)
                {
                    LogUtils.AddPluginErrorLog(pluginInfo.Id, ex);
                }
                if (metadataMenu == null) continue;
                var pluginMenu = GetMenu(pluginInfo.Id, siteId, metadataMenu, 0);

                menus.Add(pluginInfo.Id, pluginMenu);
            }

            return menus;
        }

        //public static List<ContentModelInfo> GetAllContentModels(PublishmentSystemInfo publishmentSystemInfo)
        //{
        //    var cacheName = nameof(GetAllContentModels) + publishmentSystemInfo.PublishmentSystemId;
        //    var contentModels = GetCache<List<ContentModelInfo>>(cacheName);
        //    if (contentModels != null) return contentModels;

        //    contentModels = new List<ContentModelInfo>();

        //    foreach (var pluginInfo in GetEnabledPluginInfoLists<IContentModel>())
        //    {
        //        var model = pluginInfo.Plugin as IContentModel;

        //        if (model == null) continue;

        //        var tableName = publishmentSystemInfo.AuxiliaryTableForContent;
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

        public static List<IMetadata> GetContentModelPlugins()
        {
            var list = new List<IMetadata>();

            var pluginInfoList = GetEnabledPluginInfoList<IContentModel>();
            foreach (var pluginInfo in pluginInfoList)
            {
                var plugin = (IContentModel) pluginInfo.Plugin;

                if (string.IsNullOrEmpty(plugin.ContentTableName) || plugin.ContentTableColumns == null || plugin.ContentTableColumns.Count == 0) continue;

                list.Add(pluginInfo.Metadata);
            }

            return list;
        }

        public static List<IMetadata> GetAllContentRelatedPlugins(bool includeContentTable)
        {
            var list = new List<IMetadata>();

            var pluginInfoList = GetEnabledPluginInfoList<IContentRelated>();
            foreach (var pluginInfo in pluginInfoList)
            {
                if (!includeContentTable && pluginInfo.Plugin is IContentModel) continue;

                list.Add(pluginInfo.Metadata);
            }

            return list;
        }

        public static List<IMetadata> GetContentRelatedPlugins(NodeInfo nodeInfo, bool includeContentTable)
        {
            var list = new List<IMetadata>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(nodeInfo.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(nodeInfo.ContentModelPluginId))
            {
                pluginIds.Add(nodeInfo.ContentModelPluginId);
            }

            var pluginInfoList = GetEnabledPluginInfoList<IContentRelated>();
            foreach (var pluginInfo in pluginInfoList)
            {
                var pluginId = pluginInfo.Id;
                if (!pluginIds.Contains(pluginId)) continue;

                if (!includeContentTable && pluginInfo.Plugin is IContentModel) continue;

                list.Add(pluginInfo.Metadata);
            }

            return list;
        }

        public static Dictionary<string, IContentRelated> GetContentRelatedFeatures(NodeInfo nodeInfo)
        {
            if (string.IsNullOrEmpty(nodeInfo.ContentRelatedPluginIds) &&
                string.IsNullOrEmpty(nodeInfo.ContentModelPluginId))
            {
                return new Dictionary<string, IContentRelated>();
            }

            var dict = new Dictionary<string, IContentRelated>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(nodeInfo.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(nodeInfo.ContentModelPluginId))
            {
                pluginIds.Add(nodeInfo.ContentModelPluginId);
            }

            var pluginInfoList = GetEnabledPluginInfoList<IContentRelated>();
            foreach (var pluginInfo in pluginInfoList)
            {
                var pluginId = pluginInfo.Id;
                if (!pluginIds.Contains(pluginId)) continue;

                var feature = (IContentRelated)pluginInfo.Plugin;

                dict[pluginId] = feature;
            }

            return dict;
        }

        //public static List<ContentModelInfo> GetAllContentModels(PublishmentSystemInfo publishmentSystemInfo)
        //{
        //    var cacheName = nameof(GetAllContentModels) + publishmentSystemInfo.PublishmentSystemId;
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
        //        var tableName = publishmentSystemInfo.AuxiliaryTableForContent;
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

        public static Dictionary<string, Func<PluginParseContext, string>> GetParses()
        {
            var elementsToParse = new Dictionary<string, Func<PluginParseContext, string>>();

            var plugins = GetEnabledFeatures<IParse>();
            if (plugins != null && plugins.Count > 0)
            {
                foreach (var plugin in plugins)
                {
                    if (plugin.ElementsToParse != null && plugin.ElementsToParse.Count > 0)
                    {
                        foreach (var elementName in plugin.ElementsToParse.Keys)
                        {
                            elementsToParse[elementName.ToLower()] = plugin.ElementsToParse[elementName];
                        }
                    }
                }
            }

            return elementsToParse;
        }

        public static Dictionary<string, Func<PluginRenderContext, string>> GetRenders()
        {
            var renders = new Dictionary<string, Func<PluginRenderContext, string>>();

            var pluginInfoList = GetEnabledPluginInfoList<IRender>();
            if (pluginInfoList != null && pluginInfoList.Count > 0)
            {
                foreach (var pluginInfo in pluginInfoList)
                {
                    var plugin = pluginInfo.Plugin as IRender;
                    if (plugin?.Render != null)
                    {
                        renders.Add(pluginInfo.Metadata.Id, plugin.Render);
                    }
                    //if (!(pluginInfo.Plugin is IRender plugin)) continue;

                    //if (plugin.Render != null)
                    //{
                    //    renders.Add(pluginInfo.Metadata.Id, plugin.Render);
                    //}
                }
            }

            return renders;
        }

        public static List<Action<object, FileSystemEventArgs>> GetFileSystemChangedActions()
        {
            var actions = new List<Action<object, FileSystemEventArgs>>();

            var plugins = GetEnabledFeatures<IFileSystem>();
            if (plugins != null && plugins.Count > 0)
            {
                foreach (var plugin in plugins)
                {
                    if (plugin.FileSystemChanged != null)
                    {
                        actions.Add(plugin.FileSystemChanged);
                    }
                }
            }

            return actions;
        }

        public static bool GetAndInstall(string pluginId, string version, out string errorMessage)
        {
            try
            {
                //Connect to the official package repository
                var repo = PackageRepositoryFactory.Default.CreateRepository("https://packages.nuget.org/api/v2");

                //Initialize the package manager
                var path = PathUtils.GetPackagesPath();
                var packageManager = new PackageManager(repo, path);

                //Download and unzip the package
                packageManager.InstallPackage(pluginId, SemanticVersion.Parse(version), false, true);

                var idWithVersion = $"{pluginId}.{version}";
                var directoryPath = PathUtils.Combine(path, idWithVersion);

                ZipUtils.UnpackFilesByExtension(PathUtils.Combine(directoryPath, idWithVersion + ".nupkg"), directoryPath, ".nuspec");
                
                return Install(directoryPath, out errorMessage);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static bool Install(string directoryPath, out string errorMessage)
        {
            try
            {
                string nuspecPath;
                string dllDirectoryPath;
                var metadata = GetPluginMetadataByDirectoryPath(directoryPath, out nuspecPath, out dllDirectoryPath, out errorMessage);
                if (metadata == null)
                {
                    return false;
                }

                if (IsExists(metadata.Id))
                {
                    errorMessage = $"插件 {metadata.Id} 已存在";
                    return false;
                }

                var pluginPath = PathUtils.GetPluginPath(metadata.Id);
                DirectoryUtils.CreateDirectoryIfNotExists(pluginPath);

                DirectoryUtils.Copy(PathUtils.Combine(directoryPath, "content"), pluginPath, true);
                DirectoryUtils.Copy(dllDirectoryPath, PathUtils.Combine(pluginPath, "Bin"), true);

                var configFilelPath = PathUtils.Combine(pluginPath, $"{metadata.Id}.nuspec");
                FileUtils.CopyFile(nuspecPath, configFilelPath, true);

                ClearCache();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }

            return true;
        }

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
            }
        }

        public static void UpdateTaxis(string pluginId, int taxis)
        {
            var pluginInfo = GetPluginInfo(pluginId);
            if (pluginInfo != null)
            {
                pluginInfo.Taxis = taxis;
                DataProvider.PluginDao.UpdateTaxis(pluginId, taxis);
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

        private static PluginMetadata GetPluginMetadata(string directoryName, out string dllDirectoryPath, out string errorMessage)
        {
            dllDirectoryPath = string.Empty;
            var nuspecPath = PathUtils.GetPluginNuspecPath(directoryName);
            if (!File.Exists(nuspecPath))
            {
                errorMessage = $"插件配置文件 {directoryName}.nuspec 不存在";
                return null;
            }
            dllDirectoryPath = PathUtils.GetPluginDllDirectoryPath(directoryName);
            if (string.IsNullOrEmpty(dllDirectoryPath))
            {
                errorMessage = $"插件可执行文件 {directoryName}.dll 不存在";
                return null;
            }

            PluginMetadata metadata;
            try
            {
                metadata = NuGetManager.GetPackageMetadata(nuspecPath);
            }
            catch(Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            if (string.IsNullOrEmpty(metadata.Id))
            {
                errorMessage = "插件配置文件不正确";
                return null;
            }

            errorMessage = string.Empty;
            return metadata;
        }

        public static PluginMetadata GetPluginMetadataByDirectoryPath(string directoryPath, out string nuspecPath, out string dllDirectoryPath, out string errorMessage)
        {
            nuspecPath = string.Empty;
            dllDirectoryPath = string.Empty;

            foreach (var filePath in DirectoryUtils.GetFilePaths(directoryPath))
            {
                if (StringUtils.EqualsIgnoreCase(Path.GetExtension(filePath), ".nuspec"))
                {
                    nuspecPath = filePath;
                    break;
                }
            }

            if (string.IsNullOrEmpty(nuspecPath))
            {
                errorMessage = "插件配置文件不存在";
                return null;
            }

            PluginMetadata metadata;
            try
            {
                metadata = NuGetManager.GetPackageMetadata(nuspecPath);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return null;
            }

            var pluginId = metadata.Id;

            if (string.IsNullOrEmpty(pluginId))
            {
                errorMessage = $"插件配置文件 {nuspecPath} 不正确";
                return null;
            }

            //https://docs.microsoft.com/en-us/nuget/schema/target-frameworks#supported-frameworks

            foreach (var directoryName in DirectoryUtils.GetDirectoryNames(PathUtils.Combine(directoryPath, "lib")))
            {
                if (StringUtils.StartsWithIgnoreCase(directoryName, "net45") || StringUtils.StartsWithIgnoreCase(directoryName, "net451") || StringUtils.StartsWithIgnoreCase(directoryName, "net452") || StringUtils.StartsWithIgnoreCase(directoryName, "net46") || StringUtils.StartsWithIgnoreCase(directoryName, "net461") || StringUtils.StartsWithIgnoreCase(directoryName, "net462"))
                {
                    dllDirectoryPath = PathUtils.Combine(directoryPath, "lib", directoryName);
                    break;
                }
            }
            if (string.IsNullOrEmpty(dllDirectoryPath))
            {
                dllDirectoryPath = PathUtils.Combine(directoryPath, "lib");
            }

            if (!FileUtils.IsFileExists(PathUtils.Combine(dllDirectoryPath, pluginId + ".dll")))
            {
                errorMessage = $"插件可执行文件 {pluginId}.dll 不存在";
                return null;
            }

            errorMessage = string.Empty;
            return metadata;
        }

        public static string GetPluginIconUrl(IMetadata metadata)
        {
            var url = string.Empty;
            if (metadata.IconUrl != null)
            {
                url = metadata.IconUrl.ToString();
            }
            return PageUtils.GetPluginDirectoryUrl(metadata.Id, url);
        }

        public static string GetMenuHref(string pluginId, string href, int publishmentSystemId)
        {
            if (PageUtils.IsAbsoluteUrl(href))
            {
                return href;
            }
            var url = PageUtils.AddQueryString(PageUtils.GetPluginDirectoryUrl(pluginId, href), new NameValueCollection
            {
                {"apiUrl", PageUtils.AddProtocolToUrl(PageUtility.OuterApiUrl)},
                {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
            });
            if (publishmentSystemId > 0)
            {
                url = PageUtils.AddQueryString(url, new NameValueCollection
                {
                    {"publishmentSystemId", publishmentSystemId.ToString()}
                });
            }
            return url;
        }

        public static string GetMenuContentHref(string pluginId, string href, int publishmentSystemId, int channelId, int contentId, string returnUrl)
        {
            if (PageUtils.IsAbsoluteUrl(href))
            {
                return href;
            }
            return PageUtils.AddQueryString(PageUtils.GetPluginDirectoryUrl(pluginId, href), new NameValueCollection
            {
                {"apiUrl", PageUtils.AddProtocolToUrl(PageUtility.OuterApiUrl)},
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"returnUrl", returnUrl},
                {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
            });
        }

        internal static Menu GetMenu(string pluginId, int publishmentSystemId, Menu metadataMenu, int i)
        {
            var menu = new Menu
            {
                Id = metadataMenu.Id,
                Text = metadataMenu.Text,
                Href = metadataMenu.Href,
                Target = metadataMenu.Target,
                IconClass = metadataMenu.IconClass
            };

            if (string.IsNullOrEmpty(menu.Id))
            {
                menu.Id = pluginId + i;
            }
            if (!string.IsNullOrEmpty(menu.Href))
            {
                menu.Href = GetMenuHref(pluginId, menu.Href, publishmentSystemId);
            }
            if (string.IsNullOrEmpty(menu.Target))
            {
                menu.Target = "right";
            }

            if (metadataMenu.Menus != null && metadataMenu.Menus.Count > 0)
            {
                var chlildren = new List<Menu>();
                var x = 1;
                foreach (var childMetadataMenu in metadataMenu.Menus)
                {
                    var child = GetMenu(pluginId, publishmentSystemId, childMetadataMenu, x++);

                    chlildren.Add(child);
                }
                menu.Menus = chlildren;
            }

            return menu;
        }
    }
}
