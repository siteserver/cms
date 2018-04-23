using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverTracking
    {
        [JsonProperty("trackingID")]
        public long TrackingId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("trackerType")]
        public string TrackerType { get; set; }

        [JsonProperty("lastAccessDateTime")]
        public DateTimeOffset LastAccessDateTime { get; set; }

        [JsonProperty("pageUrl")]
        public string PageUrl { get; set; }

        [JsonProperty("pageNodeID")]
        public long PageNodeId { get; set; }

        [JsonProperty("pageContentID")]
        public long PageContentId { get; set; }

        [JsonProperty("referrer")]
        public string Referrer { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("operatingSystem")]
        public string OperatingSystem { get; set; }

        [JsonProperty("browser")]
        public string Browser { get; set; }

        [JsonProperty("accessDateTime")]
        public DateTimeOffset AccessDateTime { get; set; }
    }

    public partial class SiteserverTracking
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
