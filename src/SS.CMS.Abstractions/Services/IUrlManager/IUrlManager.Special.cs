using SS.CMS.Models;

namespace SS.CMS.Services.IUrlManager
{
    public partial interface IUrlManager
    {
        string GetSpecialUrl(SiteInfo siteInfo, string url);

        string GetSpecialUrl(SiteInfo siteInfo, int specialId);

        string GetSpecialUrl(SiteInfo siteInfo, int specialId, bool isLocal);
    }
}