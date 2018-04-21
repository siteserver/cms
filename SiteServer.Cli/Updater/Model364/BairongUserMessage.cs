using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongUserMessage
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("messageFrom")]
        public string MessageFrom { get; set; }

        [JsonProperty("messageTo")]
        public string MessageTo { get; set; }

        [JsonProperty("messageType")]
        public string MessageType { get; set; }

        [JsonProperty("parentID")]
        public long ParentId { get; set; }

        [JsonProperty("isViewed")]
        public string IsViewed { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("lastAddDate")]
        public DateTimeOffset LastAddDate { get; set; }

        [JsonProperty("lastContent")]
        public string LastContent { get; set; }
    }

    public partial class BairongUserMessage
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
