using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aliyun.OSS;
using SSCMS.Configuration;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class CloudManager
    {
        public async Task RestoreAsync(string restoreId, string backupId)
        {
            _cacheManager.AddOrUpdateSliding(restoreId, 5, 100);

            var config = await _configRepository.GetAsync();
            if (config.CloudType == CloudType.Free || string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken) || config.CloudUserId == 0)
            {
                return;
            }

            var credentials = await GetOssCredentialsAsync();
            var client = new OssClient(credentials.Endpoint, credentials.AccessKeyId, credentials.AccessKeySecret, credentials.SecurityToken);

            var rootPath = _pathManager.GetRootPath();
            var storagePrefix = GetBackupPrefixKey(config.CloudUserId, backupId);
            var storageFiles = await _storageFileRepository.GetStorageFileListAsync();
            var listObjects = OssUtils.ListObjects(client, credentials.BucketName, storagePrefix);

            _cacheManager.AddOrUpdateSliding(restoreId, 10, 100);

            var current = 0;
            var total = listObjects.Count;
            var dataFilePath = string.Empty;

            foreach (var listObject in listObjects)
            {
                current++;
                var percent = 10 + (current / total) * 50;
                _cacheManager.AddOrUpdateSliding(restoreId, percent, 100);

                var storageKey = listObject.Key;
                var eTag = listObject.Value;

                var key = StringUtils.ReplaceStartsWith(storageKey, storagePrefix, string.Empty);
                var storageFile = storageFiles.FirstOrDefault(x => x.Key == key);

                var filePath = PathUtils.Combine(rootPath, key);
                var md5 = string.Empty;

                if (storageFile != null && FileUtils.IsFileExists(filePath))
                {
                    md5 = OssUtils.GetMd5(filePath);
                    if (storageFile.Md5 == md5 && storageFile.ETag == eTag)
                    {
                        continue;
                    }
                }

                if (key == CloudManager.DataZipFileName)
                {
                    var req = new GeneratePresignedUriRequest(credentials.BucketName, storageKey, SignHttpMethod.Get);
                    var pathAndQuery = client.GeneratePresignedUri(req).PathAndQuery;
                    var downloadUrl = PageUtils.Combine(CloudManager.DomainDns, pathAndQuery);

                    dataFilePath = PathUtils.Combine(_settingsManager.ContentRootPath, CloudManager.DataZipFileName);
                    FileUtils.DeleteFileIfExists(dataFilePath);
                    await RestUtils.DownloadAsync(downloadUrl, dataFilePath);
                }
                else
                {
                    var downloadUrl = PageUtils.Combine(CloudManager.DomainDns, storageKey);
                    await RestUtils.DownloadAsync(downloadUrl, filePath);

                    await _storageFileRepository.DeleteAsync(key);
                    await _storageFileRepository.InsertAsync(new StorageFile
                    {
                        Key = key,
                        ETag = eTag,
                        Md5 = md5
                    });
                }
            }

            _cacheManager.AddOrUpdateSliding(restoreId, 60, 100);

            if (string.IsNullOrEmpty(dataFilePath))
            {
                _cacheManager.AddOrUpdateSliding(restoreId, 100, 100);
                return;
            }

            var console = new FakeConsoleUtils();
            var tree = new Tree(_settingsManager, "sscms-data");
            DirectoryUtils.DeleteDirectoryIfExists(tree.DirectoryPath);
            _pathManager.ExtractZip(dataFilePath, tree.DirectoryPath);

            var errorLogFilePath = PathUtils.Combine(tree.DirectoryPath, "sscms-task.error.log");
            FileUtils.DeleteFileIfExists(errorLogFilePath);

            await _databaseManager.RestoreAsync(console, null, null, tree.TablesFilePath, tree, errorLogFilePath);

            _cacheManager.AddOrUpdateSliding(restoreId, 100, 100);
        }

        public static string GetBackupPrefixKey(int userId, string backupId)
        {
            return $"backups/{userId}/{backupId}/";
        }

        public int GetRestoreProgress(string restoreId)
        {
            var progress = _cacheManager.Get<int>(restoreId);
            return progress;
        }
    }
}
