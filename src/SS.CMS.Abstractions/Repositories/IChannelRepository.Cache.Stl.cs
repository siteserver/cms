using System.Collections.Generic;
using System.Threading.Tasks;
using SS.CMS.Data;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IChannelRepository
    {
        void StlClearCache();

        int StlGetSiteId(int channelId);

        Task<int> StlGetSequenceAsync(int siteId, int channelId);

        Task<IList<KeyValuePair<int, ChannelInfo>>> StlGetContainerChannelListAsync(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType, ScopeType scopeType, bool isTotal);

        int StlGetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel);

        Task<IList<int>> StlGetIdListByTotalNumAsync(int siteId, int channelId, TaxisType taxisType, ScopeType scopeType, string groupChannel, string groupChannelNot, bool? isImage, int totalNum);

        ChannelInfo StlGetChannelInfoByLastAddDate(int channelId);

        ChannelInfo StlGetChannelInfoByTaxis(int channelId);
    }
}
