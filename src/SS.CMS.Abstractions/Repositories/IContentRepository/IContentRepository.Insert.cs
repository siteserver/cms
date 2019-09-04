using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IContentRepository
    {
        Task<int> InsertAsync(Site siteInfo, Channel channelInfo, Content contentInfo);

        Task<int> InsertPreviewAsync(Site siteInfo, Channel channelInfo, Content contentInfo);

        Task<int> InsertWithTaxisAsync(Site siteInfo, Channel channelInfo, Content contentInfo, int taxis);
    }
}
