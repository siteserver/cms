using SS.CMS.Abstractions.Models;

namespace SS.CMS.Abstractions.Services
{
    public partial interface IUrlManager
    {
        string GetSpecialUrl(SiteInfo siteInfo, string url);

        string GetSpecialUrl(SiteInfo siteInfo, int specialId);

        string GetSpecialUrl(SiteInfo siteInfo, int specialId, bool isLocal);
    }
}