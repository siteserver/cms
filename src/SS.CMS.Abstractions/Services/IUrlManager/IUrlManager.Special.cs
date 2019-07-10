using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Services
{
    public partial interface IUrlManager
    {
        string GetSpecialUrl(Site siteInfo, string url);

        Task<string> GetSpecialUrlAsync(Site siteInfo, int specialId);

        Task<string> GetSpecialUrlAsync(Site siteInfo, int specialId, bool isLocal);
    }
}