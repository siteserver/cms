using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverNodeGroup
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeGroupName")]
        public string NodeGroupName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class SiteserverNodeGroup
    {
        public static readonly string NewTableName = DataProvider.ChannelGroupDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.ChannelGroupDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(ChannelGroupInfo.GroupName), nameof(NodeGroupName)},
                {nameof(ChannelGroupInfo.SiteId), nameof(PublishmentSystemId)}
            };
    }
}
