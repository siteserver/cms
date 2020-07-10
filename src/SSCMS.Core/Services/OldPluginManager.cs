using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using SSCMS.Repositories;
using SSCMS.Services;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class OldPluginManager : IOldPluginManager
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ISettingsManager _settingsManager;
        private readonly IPathManager _pathManager;
        private readonly IDatabaseManager _databaseManager;
        private readonly IPluginConfigRepository _pluginConfigRepository;
        private readonly IErrorLogRepository _errorLogRepository;

        public OldPluginManager(IServiceProvider serviceProvider, ISettingsManager settingsManager, IPathManager pathManager, IDatabaseManager databaseManager, IPluginConfigRepository pluginConfigRepository, IErrorLogRepository errorLogRepository)
        {
            _serviceProvider = serviceProvider;
            _settingsManager = settingsManager;
            _pathManager = pathManager;
            _databaseManager = databaseManager;
            _pluginConfigRepository = pluginConfigRepository;
            _errorLogRepository = errorLogRepository;
        }

        public List<IOldPlugin> GetPlugins()
        {
            var list = _serviceProvider.GetServices<IOldPlugin>();
            return list.ToList();
        }

        public bool IsExists(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return false;

            var plugins = GetPlugins();
            return plugins.Exists(x => StringUtils.EqualsIgnoreCase(x.PluginId, pluginId));
        }

        public IOldPlugin GetPlugin(string pluginId)
        {
            if (string.IsNullOrEmpty(pluginId)) return null;

            var plugins = GetPlugins();
            return plugins.FirstOrDefault(x => StringUtils.EqualsIgnoreCase(x.PluginId, pluginId));
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



        //public List<ContentModelInfo> GetAllContentModels(SiteInfo siteInfo)
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



        //public List<ContentModelInfo> GetAllContentModels(SiteInfo siteInfo)
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



        //public Dictionary<string, Func<PluginRenderContext, string>> GetRenders()
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

        //public List<Action<object, FileSystemEventArgs>> GetFileSystemChangedActions()
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



        //public bool Install(string pluginId, string version, out string errorMessage)
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

        public void Delete(string pluginId)
        {
            DirectoryUtils.DeleteDirectoryIfExists(_pathManager.GetPluginPath(pluginId));
        }

        //public async Task UpdateDisabledAsync(string pluginId, bool isDisabled)
        //{
        //    var plugin = GetPlugin(pluginId);
        //    if (plugin != null)
        //    {
        //        //pluginInfo.IsDisabled = isDisabled;
        //        await _databaseManager.PluginRepository.UpdateIsDisabledAsync(pluginId, isDisabled);
        //    }
        //}

        //public async Task UpdateTaxisAsync(string pluginId, int taxis)
        //{
        //    var plugin = GetPlugin(pluginId);
        //    if (plugin != null)
        //    {
        //        //pluginInfo.Taxis = taxis;
        //        await _databaseManager.PluginRepository.UpdateTaxisAsync(pluginId, taxis);
        //    }
        //}

        //public PluginMetadata UpdateDatabase(string pluginId, string databaseType, string connectionString)
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
        //internal PluginMetadata GetMetadataFromJson(string directoryPath)
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

        //private PackageMetadata GetPluginMetadata(string directoryName, out string dllDirectoryPath, out string errorMessage)
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

        public string GetPluginIconUrl(IOldPlugin pluginService)
        {
            var url = string.Empty;
            if (pluginService.IconUrl != null)
            {
                url = pluginService.IconUrl.ToString();
            }
            return url;
        }
    }
}
