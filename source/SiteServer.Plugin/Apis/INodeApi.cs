using SiteServer.Plugin.Models;
using System.Collections.Generic;

namespace SiteServer.Plugin.Apis
{
    public interface INodeApi
    {
        INodeInfo GetNodeInfo(int publishmentSystemId, int channelId);

        List<INodeInfo> GetNodeInfoList(int publishmentSystemId, string adminName);
    }
}
