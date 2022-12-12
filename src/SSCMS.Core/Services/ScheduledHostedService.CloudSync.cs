using System.Collections.Generic;
using System.Threading.Tasks;
using Aliyun.OSS;
using SSCMS.Core.Utils;
using SSCMS.Enums;
using SSCMS.Models;

namespace SSCMS.Core.Services
{
    public partial class ScheduledHostedService
    {
        private async Task<StorageContext> CloudSyncAsync(ScheduledTask task)
        {
            var config = await _databaseManager.ConfigRepository.GetAsync();
            if (config.CloudType == CloudType.Free || string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken) || config.CloudUserId == 0)
            {
                return null;
            }

            var credentials = await _cloudManager.GetOssCredentialsAsync();
            var client = new OssClient(credentials.Endpoint, credentials.AccessKeyId, credentials.AccessKeySecret, credentials.SecurityToken);

            var rootPath = _pathManager.GetRootPath();
            var storagePrefix = $"/{config.CloudUserId}/";
            var storageFiles = await _databaseManager.StorageFileRepository.GetStorageFileListAsync();
            var listObjects = new Dictionary<string, string>();
            if (storageFiles.Count > 0)
            {
                listObjects = OssUtils.ListObjects(client, credentials.BucketName, storagePrefix);
            }

            var context = new StorageContext
            {
                Credentials = credentials,
                Client = client,
                StoragePrefix = storagePrefix,
                StorageFiles = storageFiles,
                ListObjects = listObjects,
                FileKeys = new List<string>(),
                Size = 0,
            };

            await SyncDirectoryAsync(rootPath, rootPath, context);

            await DeleteStorageKeysAsync(context);

            return context;
        }
    }
}
