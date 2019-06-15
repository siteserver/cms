using System.Collections.Specialized;
using SS.CMS.Abstractions.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class UrlManager
    {
        public string GetSiteUrl(int siteId)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            return GetSiteUrl(siteInfo, false);
        }

        public string GetSiteUrl(int siteId, string virtualPath)
        {
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            return ParseNavigationUrl(siteInfo, virtualPath, false);
        }

        public string GetSiteUrlByFilePath(string filePath)
        {
            var siteId = _pathManager.GetSiteIdByFilePath(filePath);
            var siteInfo = _siteRepository.GetSiteInfo(siteId);
            return GetSiteUrlByPhysicalPath(siteInfo, filePath, false);
        }

        public void PutImagePaths(SiteInfo siteInfo, ContentInfo contentInfo, NameValueCollection collection)
        {
            if (contentInfo == null) return;

            var imageUrl = contentInfo.ImageUrl;
            var videoUrl = contentInfo.VideoUrl;
            var fileUrl = contentInfo.FileUrl;
            var content = contentInfo.Content;

            if (!string.IsNullOrEmpty(imageUrl) && IsVirtualUrl(imageUrl))
            {
                collection[imageUrl] = _pathManager.MapPath(siteInfo, imageUrl);
            }
            if (!string.IsNullOrEmpty(videoUrl) && IsVirtualUrl(videoUrl))
            {
                collection[videoUrl] = _pathManager.MapPath(siteInfo, videoUrl);
            }
            if (!string.IsNullOrEmpty(fileUrl) && IsVirtualUrl(fileUrl))
            {
                collection[fileUrl] = _pathManager.MapPath(siteInfo, fileUrl);
            }

            var srcList = RegexUtils.GetOriginalImageSrcs(content);
            foreach (var src in srcList)
            {
                if (IsVirtualUrl(src))
                {
                    collection[src] = _pathManager.MapPath(siteInfo, src);
                }
            }
        }
    }
}