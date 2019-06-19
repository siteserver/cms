using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class UrlManager
    {
        public string GetSpecialUrl(SiteInfo siteInfo, string url)
        {
            var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
            return ParseNavigationUrl(siteInfo, virtualPath, false);
        }

        public string GetSpecialUrl(SiteInfo siteInfo, int specialId)
        {
            var specialInfo = _specialRepository.GetSpecialInfo(siteInfo.Id, specialId);
            return GetSpecialUrl(siteInfo, specialInfo.Url);
        }

        public string GetSpecialUrl(SiteInfo siteInfo, int specialId, bool isLocal)
        {
            var specialUrl = GetSpecialUrl(siteInfo, specialId);

            var url = isLocal
                ? GetPreviewSpecialUrl(siteInfo.Id, specialId)
                : ParseNavigationUrl(siteInfo, specialUrl, false);

            return RemoveDefaultFileName(siteInfo, url);
        }
    }
}