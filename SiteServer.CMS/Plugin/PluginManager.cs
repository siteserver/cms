using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using BaiRong.Core;
using BaiRong.Core.IO;
using BaiRong.Core.Model.Enumerations;
using BaiRong.Core.Net;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Security;
using SiteServer.CMS.Model;
using SiteServer.CMS.Plugin.Apis;
using SiteServer.Plugin;
using SiteServer.Plugin.Features;
using SiteServer.Plugin.Models;

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

            private static SortedList<string, PluginPair> Load()
            {
                Environment = new PluginEnvironment(EDatabaseTypeUtils.GetValue(WebConfigUtils.DatabaseType), WebConfigUtils.ConnectionString,
                WebConfigUtils.PhysicalApplicationPath);
                var dict = new SortedList<string, PluginPair>();

                Thread.Sleep(2000);

                try
                {
                    var pluginsPath = PathUtils.GetPluginsPath();
                    if (!Directory.Exists(pluginsPath))
                    {
                        Directory.CreateDirectory(pluginsPath);
                    }

                    var directoryPaths = DirectoryUtils.GetDirectoryPaths(pluginsPath);
                    foreach (var directoryPath in directoryPaths)
                    {
                        var pluginPair = ActivePlugin(directoryPath);
                        if (pluginPair != null)
                        {
                            dict[pluginPair.Metadata.Id] = pluginPair;
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

            private static PluginPair ActivePlugin(string directoryPath)
            {
                //if (directoryPath.IndexOf("image-poll") == -1) return;

                try
                {
                    var metadata = GetMetadataFromJson(directoryPath);
                    if (metadata == null)
                    {
                        return null;
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

                    foreach (var filePath in DirectoryUtils.GetFilePaths(DirectoryUtils.GetDirectoryPath(metadata.ExecuteFilePath)))
                    {
                        if (!StringUtils.EqualsIgnoreCase(PathUtils.GetExtension(filePath), ".dll")) continue;

                        var fileName = PathUtils.GetFileName(filePath);
                        var binFilePath = PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "Bin", fileName);

                        if (!FileUtils.IsFileExists(binFilePath))
                        {
                            FileUtils.MoveFile(filePath, binFilePath, false);
                        }
                        else if (StringUtils.EqualsIgnoreCase(fileName, PathUtils.GetFileName(metadata.ExecuteFilePath)))
                        {
                            if (FileUtils.ComputeHash(filePath) != FileUtils.ComputeHash(binFilePath))
                            {
                                FileUtils.MoveFile(filePath, binFilePath, true);
                            }
                        }
                    }
                    //var assembly = Assembly.Load(File.ReadAllBytes(PathUtils.Combine(WebConfigUtils.PhysicalApplicationPath, "Bin", PathUtils.GetFileName(metadata.ExecuteFilePath))));
                    var assembly = Assembly.Load(PathUtils.GetFileNameWithoutExtension(metadata.ExecuteFilePath));

                    var type = assembly.GetTypes().First(o => o.IsClass && !o.IsAbstract && o.GetInterfaces().Contains(typeof(IPlugin)));
                    var plugin = (IPlugin)Activator.CreateInstance(type);

                    return ActiveAndAdd(metadata, plugin);
                }
                catch (Exception e)
                {
                    LogUtils.AddSystemErrorLog(e, $"插件加载：{directoryPath}");
                }

                return null;
            }

            private static PluginPair ActiveAndAdd(PluginMetadata metadata, IPlugin plugin)
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
                    DataApi = new DataApi(metadata),
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

                var milliseconds = s.ElapsedMilliseconds;

                metadata.InitTime = milliseconds;

                var pair = new PluginPair(context, plugin);

                return pair;
            }

            private static void Watcher_EventHandler(object sender, FileSystemEventArgs e)
            {
                var fullPath = e.FullPath.ToLower();
                if (!fullPath.Contains("-") || !fullPath.EndsWith(PluginConfigName) && !fullPath.EndsWith(".dll")) return;

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
                var directoryPath = DirectoryUtils.GetDirectoryPath(fullPath);

                var pluginDirectoryPath = string.Empty;
                var dirPaths = DirectoryUtils.GetDirectoryPaths(PathUtils.GetPluginsPath(string.Empty));
                foreach (var dirPath in dirPaths)
                {
                    if (!DirectoryUtils.IsInDirectory(dirPath, directoryPath)) continue;
                    pluginDirectoryPath = dirPath;
                    break;
                }
                if (string.IsNullOrEmpty(pluginDirectoryPath)) return;

                var plugin = AllPluginPairs.FirstOrDefault(pluginPair => PathUtils.IsEquals(pluginDirectoryPath, pluginPair.Metadata.DirectoryPath));
                if (plugin != null)
                {
                    Clear();
                }
            }

            private static void OnDirectoryDeleted(string fullPath)
            {
                var directoryPath = DirectoryUtils.GetDirectoryPath(fullPath);

                var isPlugin = false;
                foreach (var pluginPair in AllPluginPairs)
                {
                    if (!PathUtils.IsEquals(pluginPair.Metadata.DirectoryPath, directoryPath)) continue;
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

            public static SortedList<string, PluginPair> GetPluginSortedList()
            {
                var retval = CacheUtils.Get<SortedList<string, PluginPair>>(CacheKey);
                if (retval != null) return retval;

                lock (LockObject)
                {
                    retval = CacheUtils.Get<SortedList<string, PluginPair>>(CacheKey);
                    if (retval == null)
                    {
                        retval = Load();
                        CacheUtils.InsertHours(CacheKey, retval, 24);
                    }
                }

                return retval;
            }
        }

        internal const string PluginConfigName = "plugin.config";
        public static PluginEnvironment Environment { get; private set; }

        public static void ClearCache()
        {
            PluginManagerCache.Clear();
        }

        public static PluginMetadata GetMetadata(string pluginId)
        {
            var dict = PluginManagerCache.GetPluginSortedList();
            PluginPair pair;
            if (dict.TryGetValue(pluginId, out pair))
            {
                return pair.Metadata.Copy();
            }
            return null;
        }

        public static bool IsExists(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return false;

            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.ContainsKey(pluginId);
        }

        public static List<PluginPair> AllPluginPairs
        {
            get
            {
                var dict = PluginManagerCache.GetPluginSortedList();
                return dict.Values.Where(pluginPair => pluginPair?.Metadata != null && pluginPair.Plugin != null).ToList();
            }
        }

        public static List<PluginPair> GetEnabledPluginPairs<T>() where T : IPlugin
        {
            var dict = PluginManagerCache.GetPluginSortedList();
            return
                    dict.Values.Where(
                            pair =>
                                pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                                pair.Plugin is T
                        )
                        .ToList();
        }

        public static PluginPair GetEnabledPluginPair<T>(string pluginId) where T : IPlugin
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginPair pair;
            var isGet = dict.TryGetValue(pluginId, out pair);
            if (isGet && pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                pair.Plugin is T)
            {
                return pair;
            }
            return null;
        }

        public static List<PluginPair> GetEnabledPluginPairs<T1, T2>()
        {
            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.Values.Where(
                            pair =>
                                pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                                (pair.Plugin is T1 || pair.Plugin is T2)
                        )
                        .ToList();
        }

        public static List<PluginMetadata> GetEnabledPluginMetadatas<T>() where T : IPlugin
        {
            var dict = PluginManagerCache.GetPluginSortedList();

            return dict.Values.Where(
                        pair =>
                            pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                            pair.Plugin is T
                    ).Select(pluginPair => pluginPair.Metadata).ToList();
        }

        public static PluginMetadata GetEnabledPluginMetadata<T>(string pluginId) where T : IPlugin
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginPair pair;
            var isGet = dict.TryGetValue(pluginId, out pair);
            if (isGet && pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                pair.Plugin is T)
            {
                return pair.Metadata;
            }
            return null;
        }

        public static T GetEnabledFeature<T>(string pluginId) where T : IPlugin
        {
            if (string.IsNullOrEmpty(pluginId)) return default(T);

            var dict = PluginManagerCache.GetPluginSortedList();

            PluginPair pair;
            var isGet = dict.TryGetValue(pluginId, out pair);
            if (isGet && pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                pair.Plugin is T)
            {
                return (T)pair.Plugin;
            }
            return default(T);
        }

        public static List<T> GetEnabledFeatures<T>() where T : IPlugin
        {
            var dict = PluginManagerCache.GetPluginSortedList();

            var pairs = dict.Values.Where(
                        pair =>
                            pair?.Metadata != null && pair.Plugin != null && !pair.Metadata.Disabled &&
                            pair.Plugin is T
                    )
                    .ToList();
            return pairs.Select(pluginPair => (T)pluginPair.Plugin).ToList();
        }

        public static List<PermissionConfig> GetTopPermissions()
        {
            var permissions = new List<PermissionConfig>();
            foreach (var pluginPair in GetEnabledPluginPairs<IMenu>())
            {
                var feature = pluginPair.Plugin as IMenu;
                if (feature?.PluginMenu == null) continue;
                permissions.Add(new PermissionConfig(pluginPair.Metadata.Id, $"系统管理 -> {pluginPair.Metadata.DisplayName}（插件）"));
            }

            return permissions;
        }

        public static List<PermissionConfig> GetSitePermissions(int siteId)
        {
            var pairs = GetEnabledPluginPairs<IMenu>();
            var permissions = new List<PermissionConfig>();

            foreach (var pluginPair in pairs)
            {
                var feature = pluginPair.Plugin as IMenu;
                if (feature?.SiteMenu == null) continue;
                permissions.Add(new PermissionConfig(pluginPair.Metadata.Id, $"{pluginPair.Metadata.DisplayName}（插件）"));
            }

            return permissions;
        }

        public static Dictionary<string, PluginMenu> GetTopMenus()
        {
            var menus = new Dictionary<string, PluginMenu>();

            var pairs = GetEnabledPluginPairs<IMenu>();
            if (pairs != null && pairs.Count > 0)
            {
                foreach (var pluginPair in pairs)
                {
                    var feature = pluginPair.Plugin as IMenu;
                    if (feature?.PluginMenu == null) continue;

                    var pluginMenu = GetMenu(pluginPair.Metadata.Id, 0, feature.PluginMenu, 0);

                    menus.Add(pluginPair.Metadata.Id, pluginMenu);
                }
            }

            return menus;
        }

        public static Dictionary<string, PluginMenu> GetSiteMenus(int siteId)
        {
            var pairs = GetEnabledPluginPairs<IMenu>();
            if (pairs == null || pairs.Count == 0) return null;

            var menus = new Dictionary<string, PluginMenu>();

            foreach (var pluginPair in pairs)
            {
                var feature = pluginPair.Plugin as IMenu;

                PluginMenu metadataMenu = null;
                try
                {
                    metadataMenu = feature?.SiteMenu?.Invoke(siteId);
                }
                catch (Exception ex)
                {
                    LogUtils.AddPluginErrorLog(pluginPair.Metadata.Id, ex);
                }
                if (metadataMenu == null) continue;
                var pluginMenu = GetMenu(pluginPair.Metadata.Id, siteId, metadataMenu, 0);

                menus.Add(pluginPair.Metadata.Id, pluginMenu);
            }

            return menus;
        }

        //public static List<ContentModelInfo> GetAllContentModels(PublishmentSystemInfo publishmentSystemInfo)
        //{
        //    var cacheName = nameof(GetAllContentModels) + publishmentSystemInfo.PublishmentSystemId;
        //    var contentModels = GetCache<List<ContentModelInfo>>(cacheName);
        //    if (contentModels != null) return contentModels;

        //    contentModels = new List<ContentModelInfo>();

        //    foreach (var pluginPair in GetEnabledPluginPairs<IContentModel>())
        //    {
        //        var model = pluginPair.Plugin as IContentModel;

        //        if (model == null) continue;

        //        var tableName = publishmentSystemInfo.AuxiliaryTableForContent;
        //        var tableType = EAuxiliaryTableType.BackgroundContent;
        //        if (model.ContentTableColumns != null && model.ContentTableColumns.Count > 0)
        //        {
        //            tableName = pluginPair.Metadata.Id;
        //            tableType = EAuxiliaryTableType.Custom;
        //        }

        //        contentModels.Add(new ContentModelInfo(
        //            pluginPair.Metadata.Id,
        //            pluginPair.Metadata.Id,
        //            $"插件：{pluginPair.Metadata.DisplayName}",
        //            tableName,
        //            tableType,
        //            PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, pluginPair.Metadata.Icon))
        //        );
        //    }

        //    SetCache(cacheName, contentModels);

        //    return contentModels;
        //}

        public static List<PluginMetadata> GetContentModelPlugins()
        {
            var list = new List<PluginMetadata>();

            var pairs = GetEnabledPluginPairs<IContentModel>();
            foreach (var pluginPair in pairs)
            {
                var plugin = (IContentModel) pluginPair.Plugin;

                if (string.IsNullOrEmpty(plugin.ContentTableName) || plugin.ContentTableColumns == null || plugin.ContentTableColumns.Count == 0) continue;

                list.Add(pluginPair.Metadata);
            }

            return list;
        }

        public static List<PluginMetadata> GetAllContentRelatedPlugins(bool includeContentTable)
        {
            var list = new List<PluginMetadata>();

            var pairs = GetEnabledPluginPairs<IContentRelated>();
            foreach (var pluginPair in pairs)
            {
                if (!includeContentTable && pluginPair.Plugin is IContentModel) continue;

                list.Add(pluginPair.Metadata);
            }

            return list;
        }

        public static List<PluginMetadata> GetContentRelatedPlugins(NodeInfo nodeInfo, bool includeContentTable)
        {
            var list = new List<PluginMetadata>();
            var pluginIds = TranslateUtils.StringCollectionToStringList(nodeInfo.ContentRelatedPluginIds);
            if (!string.IsNullOrEmpty(nodeInfo.ContentModelPluginId))
            {
                pluginIds.Add(nodeInfo.ContentModelPluginId);
            }

            var pairs = GetEnabledPluginPairs<IContentRelated>();
            foreach (var pluginPair in pairs)
            {
                var pluginId = pluginPair.Metadata.Id;
                if (!pluginIds.Contains(pluginId)) continue;

                if (!includeContentTable && pluginPair.Plugin is IContentModel) continue;

                list.Add(pluginPair.Metadata);
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

            var pairs = GetEnabledPluginPairs<IContentRelated>();
            foreach (var pluginPair in pairs)
            {
                var pluginId = pluginPair.Metadata.Id;
                if (!pluginIds.Contains(pluginId)) continue;

                var feature = (IContentRelated)pluginPair.Plugin;

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

        //    foreach (var pluginPair in GetEnabledPluginPairs<IContentModel>())
        //    {
        //        var model = pluginPair.Plugin as IContentModel;

        //        if (model == null) continue;

        //        var links = new List<PluginContentLink>();
        //        if (model.ContentLinks != null)
        //        {
        //            links.AddRange(model.ContentLinks.Select(link => new PluginContentLink
        //            {
        //                Text = link.Text,
        //                Href = PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, link.Href),
        //                Target = link.Target
        //            }));
        //        }
        //        var tableName = publishmentSystemInfo.AuxiliaryTableForContent;
        //        var tableType = EAuxiliaryTableType.BackgroundContent;
        //        if (model.IsCustomContentTable && model.CustomContentTableColumns != null && model.CustomContentTableColumns.Count > 0)
        //        {
        //            tableName = pluginPair.Metadata.Id;
        //            tableType = EAuxiliaryTableType.Custom;
        //        }

        //        contentModels.Add(new ContentModelInfo(
        //            pluginPair.Metadata.Id,
        //            pluginPair.Metadata.Id,
        //            $"插件：{pluginPair.Metadata.DisplayName}",
        //            tableName,
        //            tableType,
        //            PageUtils.GetPluginDirectoryUrl(pluginPair.Metadata.Id, pluginPair.Metadata.Icon),
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

            var pairs = GetEnabledPluginPairs<IRender>();
            if (pairs != null && pairs.Count > 0)
            {
                foreach (var pair in pairs)
                {
                    var plugin = pair.Plugin as IRender;
                    if (plugin?.Render != null)
                    {
                        renders.Add(pair.Metadata.Id, plugin.Render);
                    }
                    //if (!(pair.Plugin is IRender plugin)) continue;

                    //if (plugin.Render != null)
                    //{
                    //    renders.Add(pair.Metadata.Id, plugin.Render);
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

        internal static string GetDownloadUrl(string pluginId, string version)
        {
            return $"http://plugins.siteserver.cn/download/{pluginId}.zip?version={version}";
        }

        public static bool Install(string pluginId, string version, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrEmpty(pluginId)) return false;

            try
            {
                if (IsExists(pluginId))
                {
                    errorMessage = "插件已存在";
                    return false;
                }
                var directoryPath = PathUtils.GetPluginsPath(pluginId);
                DirectoryUtils.DeleteDirectoryIfExists(directoryPath);

                var zipFilePath = PathUtility.GetTemporaryFilesPath(pluginId + ".zip");
                FileUtils.DeleteFileIfExists(zipFilePath);

                var downloadUrl = GetDownloadUrl(pluginId, version);
                WebClientUtils.SaveRemoteFileToLocal(downloadUrl, zipFilePath);
                
                ZipUtils.UnpackFiles(zipFilePath, directoryPath);
                FileUtils.DeleteFileIfExists(zipFilePath);

                var jsonPath = PathUtils.Combine(directoryPath, PluginConfigName);
                if (!FileUtils.IsFileExists(jsonPath))
                {
                    errorMessage = $"插件配置文件{PluginConfigName}不存在";
                    return false;
                }

                var metadata = GetMetadataFromJson(directoryPath);
                if (metadata == null)
                {
                    errorMessage = "插件配置文件不正确";
                    return false;
                }

                metadata.Disabled = false;
                metadata.DatabaseType = string.Empty;
                metadata.ConnectionString = string.Empty;

                SaveMetadataToJson(metadata);
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
                return false;
            }

            return true;
        }

        public static PluginMetadata Delete(string pluginId)
        {
            var metadata = GetMetadata(pluginId);
            if (metadata != null)
            {
                DirectoryUtils.DeleteDirectoryIfExists(metadata.DirectoryPath);
            }
            return metadata;
        }

        public static PluginMetadata UpdateDisabled(string pluginId, bool isDisabled)
        {
            var metadata = GetMetadata(pluginId);
            if (metadata != null)
            {
                metadata.Disabled = isDisabled;
                SaveMetadataToJson(metadata);
            }
            return metadata;
        }

        public static PluginMetadata UpdateDatabase(string pluginId, string databaseType, string connectionString)
        {
            var metadata = GetMetadata(pluginId);
            if (metadata != null)
            {
                if (WebConfigUtils.IsProtectData && !string.IsNullOrEmpty(databaseType))
                {
                    databaseType = TranslateUtils.EncryptStringBySecretKey(databaseType);
                }
                if (WebConfigUtils.IsProtectData && !string.IsNullOrEmpty(connectionString))
                {
                    connectionString = TranslateUtils.EncryptStringBySecretKey(connectionString);
                }
                metadata.DatabaseType = databaseType;
                metadata.ConnectionString = connectionString;
                SaveMetadataToJson(metadata);
            }
            return metadata;
        }

        /// <summary>
        /// Parse plugin metadata in giving directories
        /// </summary>
        /// <returns></returns>
        internal static PluginMetadata GetMetadataFromJson(string directoryPath)
        {
            var configPath = Path.Combine(directoryPath, PluginConfigName);
            if (!File.Exists(configPath))
            {
                return null;
            }

            PluginMetadata metadata;
            try
            {
                metadata = JsonConvert.DeserializeObject<PluginMetadata>(File.ReadAllText(configPath));
                metadata.DirectoryPath = directoryPath;
            }
            catch
            {
                return null;
            }

            if (string.IsNullOrEmpty(metadata.Id) || string.IsNullOrEmpty(metadata.Publisher) ||
                string.IsNullOrEmpty(metadata.Name) || string.IsNullOrEmpty(metadata.ExecuteFilePath) ||
                !File.Exists(metadata.ExecuteFilePath))
            {
                return null;
            }

            return metadata;
        }

        internal static bool SaveMetadataToJson(PluginMetadata metadata)
        {
            var retval = true;
            var configPath = Path.Combine(metadata.DirectoryPath, PluginConfigName);

            try
            {
                var settings = new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                };
                var json = JsonConvert.SerializeObject(metadata, Formatting.Indented, settings);
                FileUtils.WriteText(configPath, ECharset.utf_8, json);
            }
            catch (Exception ex)
            {
                retval = false;
                LogUtils.AddPluginErrorLog(metadata.Id, ex);
            }

            return retval;
        }

        public static string GetMenuHref(string pluginId, string href, int publishmentSystemId)
        {
            if (PageUtils.IsAbsoluteUrl(href))
            {
                return href;
            }
            var url = PageUtils.AddQueryString(PageUtils.GetPluginDirectoryUrl(pluginId, href), new NameValueCollection
            {
                {"apiUrl", PageUtils.AddProtocolToUrl(PageUtils.OuterApiUrl)},
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
                {"apiUrl", PageUtils.AddProtocolToUrl(PageUtils.OuterApiUrl)},
                {"publishmentSystemId", publishmentSystemId.ToString()},
                {"channelId", channelId.ToString()},
                {"contentId", contentId.ToString()},
                {"returnUrl", returnUrl},
                {"v", StringUtils.GetRandomInt(1, 1000).ToString()}
            });
        }

        internal static PluginMenu GetMenu(string pluginId, int publishmentSystemId, PluginMenu metadataMenu, int i)
        {
            var menu = new PluginMenu
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
                var chlildren = new List<PluginMenu>();
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
