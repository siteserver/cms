using System.Collections.Generic;
using SS.CMS.Data;
using SS.CMS.Models;

namespace SS.CMS.Repositories
{
    public interface IChannelGroupRepository : IRepository
    {
        int Insert(ChannelGroupInfo groupInfo);

        bool Update(ChannelGroupInfo groupInfo);

        void Delete(int siteId, string groupName);

        void UpdateTaxisToUp(int siteId, string groupName);

        void UpdateTaxisToDown(int siteId, string groupName);

        Dictionary<int, List<ChannelGroupInfo>> GetAllChannelGroups();

        bool IsExists(int siteId, string groupName);

        ChannelGroupInfo GetChannelGroupInfo(int siteId, string groupName);

        List<string> GetGroupNameList(int siteId);

        List<ChannelGroupInfo> GetChannelGroupInfoList(int siteId);
    }
}
