using System;
using System.Collections.Generic;
using BaiRong.Core;
using BaiRong.Core.Data;
using BaiRong.Core.Model.Enumerations;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Core.Permissions;
using SiteServer.Plugin.Models;

namespace SiteServer.CMS.Plugin
{
    public class PublicApiInstance : IPublicApi
    {
        private readonly PluginMetadata _metadata;

        public PublicApiInstance(PluginMetadata metadata)
        {
            _metadata = metadata;
        }

        private string _databaseType;
        public string DatabaseType
        {
            get
            {
                if (_databaseType != null) return _databaseType;

                _databaseType = EDatabaseTypeUtils.GetValue(WebConfigUtils.DatabaseType);
                if (string.IsNullOrEmpty(_metadata.DatabaseType)) return _databaseType;

                _databaseType = _metadata.DatabaseType;
                if (WebConfigUtils.IsProtectData)
                {
                    _databaseType = TranslateUtils.DecryptStringBySecretKey(_databaseType);
                }
                return _databaseType;
            }
        }

        private string _connectionString;
        public string ConnectionString
        {
            get
            {
                if (_connectionString != null) return _connectionString;

                _connectionString = WebConfigUtils.ConnectionString;
                if (string.IsNullOrEmpty(_metadata.ConnectionString)) return _connectionString;

                _connectionString = _metadata.ConnectionString;
                if (WebConfigUtils.IsProtectData)
                {
                    _connectionString = TranslateUtils.DecryptStringBySecretKey(_connectionString);
                }
                return _connectionString;
            }
        }

        private IDbHelper _dbHelper;
        public IDbHelper DbHelper
        {
            get
            {
                if (_dbHelper != null) return _dbHelper;

                if (EDatabaseTypeUtils.Equals(DatabaseType, EDatabaseType.MySql))
                {
                    _dbHelper = new MySql();
                }
                else
                {
                    _dbHelper = new SqlServer();
                }
                return _dbHelper;
            }
        }

        public int GetPublishmentSystemIdByFilePath(string path)
        {
            var publishmentSystemInfo = PathUtility.GetPublishmentSystemInfo(path);
            return publishmentSystemInfo?.PublishmentSystemId ?? 0;
        }

        public string GetPublishmentSystemPath(int publishmentSystemId)
        {
            if (publishmentSystemId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return publishmentSystemInfo == null ? null : PathUtility.GetPublishmentSystemPath(publishmentSystemInfo);
        }

        public void AddErrorLog(Exception ex)
        {
            LogUtils.AddErrorLog(ex, $"插件： {_metadata.Name}");
        }

        public List<int> GetPublishmentSystemIds()
        {
            return PublishmentSystemManager.GetPublishmentSystemIdList();
        }

        public bool SetConfig(int publishmentSystemId, object config)
        {
            return SetConfig(publishmentSystemId, string.Empty, config);
        }

        public bool SetConfig(int publishmentSystemId, string name, object config)
        {
            if (name == null) name = string.Empty;

            try
            {
                if (config == null)
                {
                    DataProvider.PluginConfigDao.Delete(_metadata.Id, publishmentSystemId, name);
                }
                else
                {
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    };
                    var json = JsonConvert.SerializeObject(config, Formatting.Indented, settings);
                    if (DataProvider.PluginConfigDao.IsExists(_metadata.Id, publishmentSystemId, name))
                    {
                        DataProvider.PluginConfigDao.Update(_metadata.Id, publishmentSystemId, name, json);
                    }
                    else
                    {
                        DataProvider.PluginConfigDao.Insert(_metadata.Id, publishmentSystemId, name, json);
                    }
                }
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
                return false;
            }
            return true;
        }

        public T GetConfig<T>(int publishmentSystemId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                var value = DataProvider.PluginConfigDao.GetValue(_metadata.Id, publishmentSystemId, name);
                if (!string.IsNullOrEmpty(value))
                {
                    return JsonConvert.DeserializeObject<T>(value);
                }
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
            }
            return default(T);
        }

        public bool RemoveConfig(int publishmentSystemId, string name = "")
        {
            if (name == null) name = string.Empty;

            try
            {
                DataProvider.PluginConfigDao.Delete(_metadata.Id, publishmentSystemId, name);
            }
            catch (Exception ex)
            {
                AddErrorLog(ex);
                return false;
            }
            return true;
        }

        public bool SetGlobalConfig(object config)
        {
            return SetConfig(0, string.Empty, config);
        }

        public bool SetGlobalConfig(string name, object config)
        {
            return SetConfig(0, name, config);
        }

        public T GetGlobalConfig<T>(string name = "")
        {
            if (name == null) name = string.Empty;
            return GetConfig<T>(0, name);
        }

        public bool RemoveGlobalConfig(string name)
        {
            return RemoveConfig(0, name);
        }

        public void MoveFiles(int sourcePublishmentSystemId, int targetPublishmentSystemId, List<string> relatedUrls)
        {
            if (sourcePublishmentSystemId == targetPublishmentSystemId) return;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(sourcePublishmentSystemId);
            var targetPublishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(targetPublishmentSystemId);
            if (publishmentSystemInfo == null || targetPublishmentSystemInfo == null) return;

            foreach (var relatedUrl in relatedUrls)
            {
                if (!string.IsNullOrEmpty(relatedUrl) && !PageUtils.IsProtocolUrl(relatedUrl))
                {
                    FileUtility.MoveFile(publishmentSystemInfo, targetPublishmentSystemInfo, relatedUrl);
                }
            }
        }

        public bool IsAuthorized()
        {
            var body = new RequestBody();
            return PermissionsManager.HasAdministratorPermissions(body.AdministratorName, _metadata.Id);
        }

        public bool IsAuthorized(int publishmentSystemId)
        {
            var body = new RequestBody();
            return PermissionsManager.HasAdministratorPermissions(body.AdministratorName, _metadata.Id + publishmentSystemId);
        }

        public string GetUploadFilePath(int publishmentSystemId, string fileName)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var localDirectoryPath = PathUtility.GetUploadDirectoryPath(publishmentSystemInfo, PathUtils.GetExtension(fileName));
            var localFileName = PathUtility.GetUploadFileName(publishmentSystemInfo, fileName);
            return PathUtils.Combine(localDirectoryPath, localFileName);
        }

        public string GetUrlByFilePath(string filePath)
        {
            var publishmentSystemId = GetPublishmentSystemIdByFilePath(filePath);
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            return PageUtility.GetPublishmentSystemUrlByPhysicalPath(publishmentSystemInfo, filePath);
        }

        public string GetPluginPageUrl(int publishmentSystemId, string relatedUrl = "")
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var apiUrl = PageUtility.GetOuterApiUrl(publishmentSystemInfo);
            return PageUtility.GetSiteFilesUrl(apiUrl, PageUtils.Combine(DirectoryUtils.SiteFiles.Plugins, _metadata.Id, relatedUrl));
        }

        public string GetPluginJsonApiUrl(int publishmentSystemId, string name = "", int id = 0)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var apiUrl = PageUtility.GetOuterApiUrl(publishmentSystemInfo);
            return Controllers.Plugins.JsonApi.GetUrl(apiUrl, _metadata.Id, name, id);
        }

        public string GetPluginHttpApiUrl(int publishmentSystemId, string name = "", int id = 0)
        {
            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var apiUrl = PageUtility.GetOuterApiUrl(publishmentSystemInfo);
            return Controllers.Plugins.HttpApi.GetUrl(apiUrl, _metadata.Id, name, id);
        }

        public IPublishmentSystemInfo GetPublishmentSystemInfo(int publishmentSystemId)
        {
            return PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
        }

        public INodeInfo GetNodeInfo(int publishmentSystemId, int channelId)
        {
            return NodeManager.GetNodeInfo(publishmentSystemId, channelId);
        }

        public IContentInfo GetContentInfo(int publishmentSystemId, int channelId, int contentId)
        {
            if (publishmentSystemId <= 0 || channelId <= 0 || contentId <= 0) return null;

            var publishmentSystemInfo = PublishmentSystemManager.GetPublishmentSystemInfo(publishmentSystemId);
            var tableStyle = NodeManager.GetTableStyle(publishmentSystemInfo, channelId);
            var tableName = NodeManager.GetTableName(publishmentSystemInfo, channelId);

            return DataProvider.ContentDao.GetContentInfo(tableStyle, tableName, contentId);
        }
    }
}
