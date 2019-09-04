using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface IContentRepository
    {
        Task<int> GetCountAsync(Site siteInfo, bool isChecked, IPluginManager pluginManager);

        Task<int> GetCountAsync(Site siteInfo, Channel channelInfo, int? onlyAdminId);

        Task<int> GetCountAsync(Site siteInfo, Channel channelInfo, bool isChecked);
    }
}