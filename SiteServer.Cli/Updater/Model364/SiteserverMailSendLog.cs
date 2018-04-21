using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverMailSendLog
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("channelID")]
        public long ChannelId { get; set; }

        [JsonProperty("contentID")]
        public long ContentId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("pageUrl")]
        public string PageUrl { get; set; }

        [JsonProperty("receiver")]
        public string Receiver { get; set; }

        [JsonProperty("mail")]
        public string Mail { get; set; }

        [JsonProperty("sender")]
        public string Sender { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }
    }

    public partial class SiteserverMailSendLog
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
