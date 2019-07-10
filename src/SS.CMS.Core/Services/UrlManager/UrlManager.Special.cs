using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Utils;

namespace SS.CMS.Core.Services
{
    public partial class UrlManager
    {
        public string GetSpecialUrl(Site siteInfo, string url)
        {
            var virtualPath = PageUtils.RemoveFileNameFromUrl(url);
            return ParseNavigationUrl(siteInfo, virtualPath, false);
        }

        public async Task<string> GetSpecialUrlAsync(Site siteInfo, int specialId)
        {
            var specialInfo = await _specialRepository.GetSpecialInfoAsync(siteInfo.Id, specialId);
            return GetSpecialUrl(siteInfo, specialInfo.Url);
        }

        public async Task<string> GetSpecialUrlAsync(Site siteInfo, int specialId, bool isLocal)
        {
            var specialUrl = await GetSpecialUrlAsync(siteInfo, specialId);

            var url = isLocal
                ? GetPreviewSpecialUrl(siteInfo.Id, specialId)
                : ParseNavigationUrl(siteInfo, specialUrl, false);

            return RemoveDefaultFileName(siteInfo, url);
        }
    }
}