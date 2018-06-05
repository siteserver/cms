using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model40
{
    public partial class TableSiteLog
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("publishmentSystemId")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("channelId")]
        public long ChannelId { get; set; }

        [JsonProperty("contentId")]
        public long ContentId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }
    }

    public partial class TableSiteLog
    {
        public const string OldTableName = "Log";

        public static readonly string NewTableName = DataProvider.SiteLogDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.SiteLogDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(SiteLogInfo.SiteId), nameof(PublishmentSystemId)}
            };
    }
}
