using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverSeoMeta
    {
        [JsonProperty("seoMetaID")]
        public long SeoMetaId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("seoMetaName")]
        public string SeoMetaName { get; set; }

        [JsonProperty("isDefault")]
        public string IsDefault { get; set; }

        [JsonProperty("pageTitle")]
        public string PageTitle { get; set; }

        [JsonProperty("keywords")]
        public string Keywords { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("copyright")]
        public string Copyright { get; set; }

        [JsonProperty("author")]
        public string Author { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("charset")]
        public string Charset { get; set; }

        [JsonProperty("distribution")]
        public string Distribution { get; set; }

        [JsonProperty("rating")]
        public string Rating { get; set; }

        [JsonProperty("robots")]
        public string Robots { get; set; }

        [JsonProperty("revisitAfter")]
        public string RevisitAfter { get; set; }

        [JsonProperty("expires")]
        public string Expires { get; set; }
    }

    public partial class SiteserverSeoMeta
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
