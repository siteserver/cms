using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<List<InputListItem>> GetContentsColumnsAsync(Site siteInfo, Channel channelInfo, bool includeAll);

        Task<List<ContentColumn>> GetContentColumnsAsync(Site siteInfo, Channel channelInfo, bool includeAll);
    }
}
