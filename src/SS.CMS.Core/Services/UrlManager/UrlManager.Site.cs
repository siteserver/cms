using System.Collections.Specialized;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class UrlManager
    {
        public async Task<string> GetSiteUrlAsync(int siteId)
        {
            var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            return GetSiteUrl(siteInfo, false);
        }

        public async Task<string> GetSiteUrlAsync(int siteId, string virtualPath)
        {
            var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            return ParseNavigationUrl(siteInfo, virtualPath, false);
        }

        public async Task<string> GetSiteUrlByFilePathAsync(string filePath)
        {
            var siteId = await _pathManager.GetSiteIdByFilePathAsync(filePath);
            var siteInfo = await _siteRepository.GetSiteInfoAsync(siteId);
            return GetSiteUrlByPhysicalPath(siteInfo, filePath, false);
        }

        public void PutImagePaths(Site siteInfo, Content contentInfo, NameValueCollection collection)
        {
            if (contentInfo == null) return;

            var imageUrl = contentInfo.ImageUrl;
            var videoUrl = contentInfo.VideoUrl;
            var fileUrl = contentInfo.FileUrl;
            var content = contentInfo.Body;

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