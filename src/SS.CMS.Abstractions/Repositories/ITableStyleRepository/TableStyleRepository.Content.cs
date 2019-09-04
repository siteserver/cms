using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Models;
using SS.CMS.Services;

namespace SS.CMS.Repositories
{
    public partial interface ITableStyleRepository
    {
        Task<List<TableStyle>> GetContentStyleInfoListAsync(Site site, Channel channel);

        Task<List<InputListItem>> GetContentsColumnsAsync(Site siteInfo, Channel channelInfo, bool includeAll, IPluginManager pluginManager);

        Task<List<ContentColumn>> GetContentColumnsAsync(Site siteInfo, Channel channelInfo, bool includeAll, IPluginManager pluginManager);
    }
}