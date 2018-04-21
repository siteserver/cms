using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverGatherRule
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("gatherRuleName")]
        public string GatherRuleName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("cookieString")]
        public string CookieString { get; set; }

        [JsonProperty("gatherUrlIsCollection")]
        public string GatherUrlIsCollection { get; set; }

        [JsonProperty("gatherUrlCollection")]
        public string GatherUrlCollection { get; set; }

        [JsonProperty("gatherUrlIsSerialize")]
        public string GatherUrlIsSerialize { get; set; }

        [JsonProperty("gatherUrlSerialize")]
        public string GatherUrlSerialize { get; set; }

        [JsonProperty("serializeFrom")]
        public long SerializeFrom { get; set; }

        [JsonProperty("serializeTo")]
        public long SerializeTo { get; set; }

        [JsonProperty("serializeInterval")]
        public long SerializeInterval { get; set; }

        [JsonProperty("serializeIsOrderByDesc")]
        public string SerializeIsOrderByDesc { get; set; }

        [JsonProperty("serializeIsAddZero")]
        public string SerializeIsAddZero { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("charset")]
        public string Charset { get; set; }

        [JsonProperty("urlInclude")]
        public string UrlInclude { get; set; }

        [JsonProperty("titleInclude")]
        public string TitleInclude { get; set; }

        [JsonProperty("contentExclude")]
        public string ContentExclude { get; set; }

        [JsonProperty("contentHtmlClearCollection")]
        public string ContentHtmlClearCollection { get; set; }

        [JsonProperty("contentHtmlClearTagCollection")]
        public string ContentHtmlClearTagCollection { get; set; }

        [JsonProperty("lastGatherDate")]
        public DateTimeOffset LastGatherDate { get; set; }

        [JsonProperty("listAreaStart")]
        public string ListAreaStart { get; set; }

        [JsonProperty("listAreaEnd")]
        public string ListAreaEnd { get; set; }

        [JsonProperty("contentChannelStart")]
        public string ContentChannelStart { get; set; }

        [JsonProperty("contentChannelEnd")]
        public string ContentChannelEnd { get; set; }

        [JsonProperty("contentTitleStart")]
        public string ContentTitleStart { get; set; }

        [JsonProperty("contentTitleEnd")]
        public string ContentTitleEnd { get; set; }

        [JsonProperty("contentContentStart")]
        public string ContentContentStart { get; set; }

        [JsonProperty("contentContentEnd")]
        public string ContentContentEnd { get; set; }

        [JsonProperty("contentNextPageStart")]
        public string ContentNextPageStart { get; set; }

        [JsonProperty("contentNextPageEnd")]
        public string ContentNextPageEnd { get; set; }

        [JsonProperty("contentAttributes")]
        public string ContentAttributes { get; set; }

        [JsonProperty("contentAttributesXML")]
        public string ContentAttributesXml { get; set; }

        [JsonProperty("extendValues")]
        public string ExtendValues { get; set; }
    }

    public partial class SiteserverGatherRule
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
