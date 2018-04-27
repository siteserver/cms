using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class TableSitePermissions
    {
        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeIDCollection")]
        public string NodeIdCollection { get; set; }

        [JsonProperty("channelPermissions")]
        public string ChannelPermissions { get; set; }

        [JsonProperty("websitePermissions")]
        public string WebsitePermissions { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class TableSitePermissions
    {
        public const string OldTableName = "siteserver_SystemPermissions";

        public static readonly string NewTableName = DataProvider.SitePermissionsDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.SitePermissionsDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(SitePermissionsInfo.SiteId), nameof(PublishmentSystemId)},
                {nameof(SitePermissionsInfo.ChannelIdCollection), nameof(NodeIdCollection)}
            };
    }
}
