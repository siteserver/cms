using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<int> GetCountAsync(SiteInfo siteInfo, bool isChecked);

        Task<int> GetCountAsync(SiteInfo siteInfo, ChannelInfo channelInfo, int? onlyAdminId);

        Task<int> GetCountAsync(SiteInfo siteInfo, ChannelInfo channelInfo, bool isChecked);
    }
}