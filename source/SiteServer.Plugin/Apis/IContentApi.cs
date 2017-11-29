using System.Collections.Generic;
using System.Collections.Specialized;
using SiteServer.Plugin.Models;

namespace SiteServer.Plugin.Apis
{
    public interface IContentApi
    {
        IContentInfo NewInstance(int publishmentSystemId, int channelId);

        IContentInfo GetContentInfo(int publishmentSystemId, int channelId, int contentId);

        List<IContentInfo> GetContentInfoList(int publishmentSystemId, int channelId, string whereString,
            string orderString, int limit, int offset);

        int GetCount(int publishmentSystemId, int channelId, string whereString);

        string GetContentValue(int publishmentSystemId, int channelId, int contentId, string attributeName);

        List<PluginTableColumn> GetTableColumns(int publishmentSystemId, int channelId);

        int Insert(int publishmentSystemId, int channelId, IContentInfo contentInfo);

        void Update(int publishmentSystemId, int channelId, IContentInfo contentInfo);

        void Delete(int publishmentSystemId, int channelId, int contentId);
    }
}
