using System.Threading.Tasks;

namespace SSCMS
{
    public partial interface IContentRepository
    {
        Task<int> InsertAsync(Site site, Channel channel, Content content);

        Task<int> InsertPreviewAsync(Site site, Channel channel, Content content);

        Task<int> InsertWithTaxisAsync(Site site, Channel channel, Content content, int taxis);
    }
}
