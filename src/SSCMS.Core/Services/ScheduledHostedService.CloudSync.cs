using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Aliyun.OSS;
using Aliyun.OSS.Util;
using SSCMS.Dto;
using SSCMS.Enums;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ScheduledHostedService
    {
        public class Context
        {
            public OssCredentials Credentials { get; set; }
            public OssClient Client { get; set; }
            public string RootPath { get; set; }
            public string StoragePrefix { get; set; }
            public List<StorageFile> StorageFiles { get; set; }
            public Dictionary<string, string> ListObjects { get; set; }
        }

        private async Task CloudSyncAsync(ScheduledTask task)
        {
            var config = await _configRepository.GetAsync();
            if (config.CloudType == CloudType.Free || string.IsNullOrEmpty(config.CloudUserName) || string.IsNullOrEmpty(config.CloudToken) || config.CloudUserId == 0) return;

            var credentials = await _cloudManager.GetOssCredentialsAsync();
            var client = new OssClient(credentials.Endpoint, credentials.AccessKeyId, credentials.AccessKeySecret, credentials.SecurityToken);

            var rootPath = _pathManager.GetRootPath();
            var storagePrefix = $"/{config.CloudUserId}/";
            var storageFiles = await _storageFileRepository.GetStorageFileListAsync();
            var listObjects = new Dictionary<string, string>();
            if (storageFiles.Count > 0)
            {
                listObjects = ListObjects(client, credentials.BucketName, storagePrefix);
            }

            var context = new Context
            {
                Credentials = credentials,
                Client = client,
                RootPath =rootPath,
                StoragePrefix = storagePrefix,
                StorageFiles= storageFiles,
                ListObjects =listObjects,
            };

            await AddSyncDirectoryTasksAsync(rootPath, context);
        }

        public static readonly List<string> StorageExcludes = new List<string> {
          "sitefiles", 
          "ss-admin"
        };

        private async Task AddSyncDirectoryTasksAsync(string directoryPath, Context context)
        {
            if (string.IsNullOrEmpty(directoryPath)) return;

            var key = PathUtils.GetPathDifference(context.RootPath, directoryPath);
            key = StringUtils.TrimSlash(key.Replace('\\', '/'));
            if (!string.IsNullOrEmpty(key) && ListUtils.ContainsIgnoreCase(StorageExcludes, key)) return;

            var filePaths = DirectoryUtils.GetFilePaths(directoryPath);
            foreach (var filePath in filePaths)
            {
                await AddSyncFileTaskAsync(filePath, context);
            }

            var directoryPaths = DirectoryUtils.GetDirectoryPaths(directoryPath);
            foreach (var dirPath in directoryPaths)
            {
                await AddSyncDirectoryTasksAsync(dirPath, context);
            }
        }

        public static string GetMd5(string filePath)
        {
            string md5;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                md5 = OssUtils.ComputeContentMd5(fs, fs.Length);
                fs.Close();
            }

            return md5;
        }

        private async Task AddSyncFileTaskAsync(string filePath, Context context)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            var key = PathUtils.GetPathDifference(context.RootPath, filePath);
            if (!string.IsNullOrEmpty(key))
            {
                key = StringUtils.TrimSlash(key.Replace('\\', '/'));
                if (ListUtils.ContainsIgnoreCase(StorageExcludes, key)) return;

                var md5 = GetMd5(filePath);

                if (context.ListObjects != null && context.StorageFiles != null)
                {
                    var storageFile = context.StorageFiles.FirstOrDefault(x => x.Key == key);
                    var storageKey = StringUtils.TrimSlash(PageUtils.Combine(context.StoragePrefix, key));
                    if (storageFile != null && storageFile.Md5 == md5 && context.ListObjects.ContainsKey(storageKey))
                    {
                        var eTagOfStorage = context.ListObjects[storageKey];
                        if (storageFile.ETag == eTagOfStorage)
                        {
                            return;
                        }
                    }
                }

                var putKey = StringUtils.TrimSlash(PageUtils.Combine(context.StoragePrefix, key));
                var result = context.Client.PutObject(context.Credentials.BucketName, putKey, filePath);
                var eTag = result.ETag;

                await _storageFileRepository.DeleteAsync(key);
                await _storageFileRepository.InsertAsync(new StorageFile
                {
                    Key = key,
                    ETag = eTag,
                    Md5 = md5
                });
            }
        }

        public Dictionary<string, string> ListObjects(OssClient client, string bucketName, string prefix)
        {
            var objects = new Dictionary<string, string>();
            ObjectListing result = null;
            string nextMarker = string.Empty;
            var listPrefix = SSCMS.Utils.StringUtils.TrimSlash(prefix);
            if (!string.IsNullOrEmpty(listPrefix))
            {
                listPrefix = $"{listPrefix}/";
            }
            do
            {
                var listObjectsRequest = new ListObjectsRequest(bucketName)
                {
                    Marker = nextMarker,
                    Prefix = listPrefix,
                    MaxKeys = 1000
                };
                // 列举文件。
                result = client.ListObjects(listObjectsRequest);
                foreach (var summary in result.ObjectSummaries)
                {
                    objects[summary.Key] = summary.ETag;
                }
                nextMarker = result.NextMarker;
            } while (result.IsTruncated);

            return objects;
        }
    }
}
