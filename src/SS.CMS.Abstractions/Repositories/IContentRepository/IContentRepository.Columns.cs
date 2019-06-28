using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;

namespace SS.CMS.Repositories.IContentRepository
{
    public partial interface IContentRepository
    {
        Task<List<InputListItem>> GetContentsColumnsAsync(SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll);

        Task<List<ContentColumn>> GetContentColumnsAsync(SiteInfo siteInfo, ChannelInfo channelInfo, bool includeAll);
    }
}
