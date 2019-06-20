using System.Collections.Specialized;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IUrlManager
    {
        string GetSiteUrl(int siteId);

        string GetSiteUrl(int siteId, string virtualPath);

        string GetSiteUrlByFilePath(string filePath);

        void PutImagePaths(SiteInfo siteInfo, ContentInfo contentInfo, NameValueCollection collection);
    }
}