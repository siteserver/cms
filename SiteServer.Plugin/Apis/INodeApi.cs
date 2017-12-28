using SiteServer.Plugin.Models;
using System.Collections.Generic;

namespace SiteServer.Plugin.Apis
{
    public interface INodeApi
    {
        INodeInfo NewInstance(int publishmentSystemId);

        INodeInfo GetNodeInfo(int publishmentSystemId, int channelId);

        List<int> GetNodeIdList(int publishmentSystemId, string adminName);

        List<int> GetNodeIdList(int publishmentSystemId);

        List<int> GetNodeIdList(int publishmentSystemId, int parentId);

        int Insert(int publishmentSystemId, INodeInfo nodeInfo);
    }
}
