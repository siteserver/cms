using System.Collections.Generic;
using System.IO;
using Aliyun.OSS;

namespace SSCMS.Core.Utils
{
    public static class OssUtils
    {
        public static Dictionary<string, string> ListObjects(OssClient client, string bucketName, string prefix)
        {
            var objects = new Dictionary<string, string>();
            ObjectListing result = null;
            var nextMarker = string.Empty;
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
                result = client.ListObjects(listObjectsRequest);
                foreach (var summary in result.ObjectSummaries)
                {
                    objects[summary.Key] = summary.ETag;
                }
                nextMarker = result.NextMarker;
            } while (result.IsTruncated);

            return objects;
        }

        public static (string md5, long fileLength) GetMd5AndLength(string filePath)
        {
            string md5;
            long length;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                length = fs.Length;
                md5 = Aliyun.OSS.Util.OssUtils.ComputeContentMd5(fs, fs.Length);
                fs.Close();
            }

            return (md5, length);
        }

        public static string GetMd5(string filePath)
        {
            string md5;
            using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                md5 = Aliyun.OSS.Util.OssUtils.ComputeContentMd5(fs, fs.Length);
                fs.Close();
            }

            return md5;
        }
    }
}