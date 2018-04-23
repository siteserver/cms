using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverCommentContent
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("commentID")]
        public long CommentId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("contentID")]
        public long ContentId { get; set; }

        [JsonProperty("referenceID")]
        public long ReferenceId { get; set; }

        [JsonProperty("good")]
        public long Good { get; set; }

        [JsonProperty("bad")]
        public long Bad { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("attribute1")]
        public string Attribute1 { get; set; }

        [JsonProperty("attribute2")]
        public string Attribute2 { get; set; }

        [JsonProperty("attribute3")]
        public string Attribute3 { get; set; }

        [JsonProperty("attribute4")]
        public string Attribute4 { get; set; }

        [JsonProperty("attribute5")]
        public string Attribute5 { get; set; }

        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }

    public partial class SiteserverCommentContent
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
