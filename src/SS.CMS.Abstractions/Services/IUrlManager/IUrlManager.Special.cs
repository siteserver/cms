using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IUrlManager
    {
        string GetSpecialUrl(SiteInfo siteInfo, string url);

        Task<string> GetSpecialUrlAsync(SiteInfo siteInfo, int specialId);

        Task<string> GetSpecialUrlAsync(SiteInfo siteInfo, int specialId, bool isLocal);
    }
}