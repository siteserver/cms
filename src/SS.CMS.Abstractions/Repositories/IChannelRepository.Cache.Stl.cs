using System.Collections.Generic;
using SS.CMS.Enums;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public partial interface IChannelRepository
    {
        void StlClearCache();

        int StlGetSiteId(int channelId);

        int StlGetSequence(int siteId, int channelId);

        IList<KeyValuePair<int, ChannelInfo>> StlGetContainerChannelList(int siteId, int channelId, string group, string groupNot, bool? isImage, int startNum, int totalNum, TaxisType taxisType, ScopeType scopeType, bool isTotal);

        int StlGetIdByParentIdAndTaxis(int parentId, int taxis, bool isNextChannel);

        IList<int> StlGetIdListByTotalNum(int siteId, int channelId, TaxisType taxisType, ScopeType scopeType, string groupChannel, string groupChannelNot, bool? isImage, int totalNum);

        ChannelInfo StlGetChannelInfoByLastAddDate(int channelId);

        ChannelInfo StlGetChannelInfoByTaxis(int channelId);
    }
}
