using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<int> GetCountAsync(Site siteInfo, bool isChecked);

        Task<int> GetCountAsync(Site siteInfo, Channel channelInfo, int? onlyAdminId);

        Task<int> GetCountAsync(Site siteInfo, Channel channelInfo, bool isChecked);
    }
}