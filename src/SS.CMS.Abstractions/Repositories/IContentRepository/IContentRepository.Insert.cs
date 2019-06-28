using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<int> InsertAsync(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        Task<int> InsertPreviewAsync(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo);

        Task<int> InsertWithTaxisAsync(SiteInfo siteInfo, ChannelInfo channelInfo, ContentInfo contentInfo, int taxis);
    }
}
