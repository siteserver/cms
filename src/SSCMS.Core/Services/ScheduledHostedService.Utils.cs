using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Aliyun.OSS;
using SSCMS.Core.Utils;
using SSCMS.Dto;
using SSCMS.Models;
using SSCMS.Utils;

namespace SSCMS.Core.Services
{
    public partial class ScheduledHostedService
    {
        private class StorageContext
        {
            public OssCredentials Credentials { get; set; }
            public OssClient Client { get; set; }
            public string StoragePrefix { get; set; }
            public List<StorageFile> StorageFiles { get; set; }
            public Dictionary<string, string> ListObjects { get; set; }
            public List<string> FileKeys { get; set; }
            public long Size { get; set; }
        }

        private static readonly List<string> StorageExcludes = new List<string> {
          "sitefiles/assets",
          "sitefiles/resources",
          "sitefiles/sitetemplates",
          "sitefiles/temporaryfiles",
          "ss-admin"
        };

        private async Task DeleteStorageKeysAsync(StorageContext context)
        {
            var deleteStorageKeys = new List<string>();
            var storagePrefix = StringUtils.ReplaceStartsWith(context.StoragePrefix, "/", string.Empty);
            foreach (var storageKey in context.ListObjects.Keys)
            {
                var fileKey = StringUtils.TrimSlash(StringUtils.ReplaceStartsWith(storageKey, storagePrefix, string.Empty));
                if (!string.IsNullOrEmpty(fileKey) && !context.FileKeys.Contains(fileKey))
                {
                    await _databaseManager.StorageFileRepository.DeleteAsync(fileKey);
                    deleteStorageKeys.Add(storageKey);
                }
            }

            if (deleteStorageKeys.Count == 0) return;
            for (int i = 0; i < deleteStorageKeys.Count; i += 999)
            {
                var deleteKeys = deleteStorageKeys.Skip(i).Take(999).ToList();
                var request = new DeleteObjectsRequest(context.Credentials.BucketName, deleteKeys, true);
                context.Client.DeleteObjects(request);
            }
        }

        private async Task SyncDirectoryAsync(string rootPath, string directoryPath, StorageContext context)
        {
            if (string.IsNullOrEmpty(directoryPath)) return;

            var key = PathUtils.GetPathDifference(rootPath, directoryPath);
            key = StringUtils.TrimSlash(key.Replace('\\', '/'));
            if (!string.IsNullOrEmpty(key) && ListUtils.ContainsIgnoreCase(StorageExcludes, key)) return;

            var filePaths = DirectoryUtils.GetFilePaths(directoryPath);
            foreach (var filePath in filePaths)
            {
                await SyncFileAsync(rootPath, filePath, context);
            }

            var directoryPaths = DirectoryUtils.GetDirectoryPaths(directoryPath);
            foreach (var dirPath in directoryPaths)
            {
                await SyncDirectoryAsync(rootPath, dirPath, context);
            }
        }

        private async Task<string> SyncFileAsync(string rootPath, string filePath, StorageContext context)
        {
            if (string.IsNullOrEmpty(filePath)) return string.Empty;

            var key = PathUtils.GetPathDifference(rootPath, filePath);
            if (!string.IsNullOrEmpty(key))
            {
                key = StringUtils.TrimSlash(key.Replace('\\', '/'));
                if (ListUtils.ContainsIgnoreCase(StorageExcludes, key)) return string.Empty;
                if (context.FileKeys.Contains(key)) return string.Empty;
                context.FileKeys.Add(key);

                var (md5, length) = OssUtils.GetMd5AndLength(filePath);
                context.Size += length;

                if (context.ListObjects != null && context.StorageFiles != null)
                {
                    var storageFile = context.StorageFiles.FirstOrDefault(x => x.Key == key);
                    var storageKey = StringUtils.TrimSlash(PageUtils.Combine(context.StoragePrefix, key));
                    if (storageFile != null && storageFile.Md5 == md5 && context.ListObjects.ContainsKey(storageKey))
                    {
                        var eTagOfStorage = context.ListObjects[storageKey];
                        if (storageFile.ETag == eTagOfStorage)
                        {
                            return key;
                        }
                    }
                }

                var putKey = StringUtils.TrimSlash(PageUtils.Combine(context.StoragePrefix, key));
                var result = context.Client.PutObject(context.Credentials.BucketName, putKey, filePath);
                var eTag = result.ETag;

                await _databaseManager.StorageFileRepository.DeleteAsync(key);
                await _databaseManager.StorageFileRepository.InsertAsync(new StorageFile
                {
                    Key = key,
                    ETag = eTag,
                    Md5 = md5
                });
            }

            return key;
        }
    }
}
