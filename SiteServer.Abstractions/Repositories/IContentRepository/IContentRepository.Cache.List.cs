using System.Collections.Generic;
using System.Threading.Tasks;


namespace SiteServer.Abstractions
{
    public partial interface IContentRepository
    {
        Task<IEnumerable<int>> GetContentIdListAsync(Site siteInfo, Channel channelInfo, int? onlyUserId, int offset, int limit);
    }
}