using System;
using System.IO;
using Aliyun.OSS;
using SiteServer.Plugin;

namespace Oss
{
    public class Main : IPlugin, IFileSystemWatcher, IRestful
    {
        private IPublicApi _api;
        private const string OptionIsOss = "Oss.IsOss";
        private const string OptionAccessKeyId = "Oss.AccessKeyId";
        private const string OptionAccessKeySecret = "Oss.AccessKeySecret";
        private const string OptionBucketName = "Oss.BucketName";
        private const string OptionBucketEndPoint = "Oss.BucketEndPoint";
        private const string OptionBucketPath = "Oss.BucketPath";

        private static string GetRelativePath(string filePath, string directoryPath)
        {
            var pathUri = new Uri(filePath);
            // Folders must end in a slash
            if (!directoryPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                directoryPath += Path.DirectorySeparatorChar;
            }
            var folderUri = new Uri(directoryPath);
            return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace(Path.DirectorySeparatorChar, '/'));
        }

        public void Initialize(PluginContext context)
        {
            _api = context.Api;
        }

        public void Dispose(PluginContext context)
        {
            
        }

        public void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            try
            {
                if (e.ChangeType != WatcherChangeTypes.Changed && e.ChangeType != WatcherChangeTypes.Created) return;
                if (string.IsNullOrEmpty(Path.GetExtension(e.FullPath))) return;

                var filePath = e.FullPath;
                var siteId = _api.GetSiteIdByFilePath(e.FullPath);
                if (siteId <= 0) return;
                var siteDirectoryPath = _api.GetSiteDirectoryPath(siteId);
                if (string.IsNullOrEmpty(siteDirectoryPath)) return;

                var isOss = _api.GetSiteOption(siteId, OptionIsOss) == true.ToString();
                if (!isOss) return;

                var accessKeyId = _api.GetSiteOption(siteId, OptionAccessKeyId);
                var accessKeySecret = _api.GetSiteOption(siteId, OptionAccessKeySecret);
                var bucketName = _api.GetSiteOption(siteId, OptionBucketName);
                var bucketEndPoint = _api.GetSiteOption(siteId, OptionBucketEndPoint);
                var bucketPath = _api.GetSiteOption(siteId, OptionBucketPath);
                var key = (bucketPath + GetRelativePath(filePath, siteDirectoryPath)).Trim('/');

                if (string.IsNullOrEmpty(accessKeyId) || string.IsNullOrEmpty(accessKeySecret) ||
                    string.IsNullOrEmpty(bucketName) || string.IsNullOrEmpty(bucketEndPoint) ||
                    string.IsNullOrEmpty(key)) return;

                var client = new OssClient(bucketEndPoint, accessKeyId, accessKeySecret);
                client.PutObject(bucketName, key, filePath);
            }
            catch (Exception ex)
            {
                _api.AddErrorLog(ex);
            }
        }

        public object Get(IRequestContext context)
        {
            var isOss = _api.GetSiteOption(context.SiteId, OptionIsOss) == true.ToString();

            var accessKeyId = _api.GetSiteOption(context.SiteId, OptionAccessKeyId);
            var accessKeySecret = _api.GetSiteOption(context.SiteId, OptionAccessKeySecret);
            var bucketName = _api.GetSiteOption(context.SiteId, OptionBucketName);
            var bucketEndPoint = _api.GetSiteOption(context.SiteId, OptionBucketEndPoint);
            var bucketPath = _api.GetSiteOption(context.SiteId, OptionBucketPath);

            return new
            {
                isOss,
                accessKeyId,
                accessKeySecret,
                bucketName,
                bucketEndPoint,
                bucketPath
            };
        }

        public object Post(IRequestContext context)
        {
            var isOss = context.GetPostBool("isOss");
            var accessKeyId = context.GetPostString("accessKeyId");
            var accessKeySecret = context.GetPostString("accessKeySecret");
            var bucketName = context.GetPostString("bucketName");
            var bucketEndPoint = context.GetPostString("bucketEndPoint");
            var bucketPath = context.GetPostString("bucketPath");

            _api.SetSiteOption(context.SiteId, OptionIsOss, isOss.ToString());
            _api.SetSiteOption(context.SiteId, OptionAccessKeyId, accessKeyId);
            _api.SetSiteOption(context.SiteId, OptionAccessKeySecret, accessKeySecret);
            _api.SetSiteOption(context.SiteId, OptionBucketName, bucketName);
            _api.SetSiteOption(context.SiteId, OptionBucketEndPoint, bucketEndPoint);
            _api.SetSiteOption(context.SiteId, OptionBucketPath, bucketPath);

            return null;
        }

        public object Put(IRequestContext context)
        {
            throw new NotImplementedException();
        }

        public object Delete(IRequestContext context)
        {
            throw new NotImplementedException();
        }

        public object Patch(IRequestContext context)
        {
            throw new NotImplementedException();
        }
    }
}