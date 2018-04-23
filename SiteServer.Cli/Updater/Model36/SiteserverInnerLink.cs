using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverInnerLink
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("innerLinkName")]
        public string InnerLinkName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("linkUrl")]
        public string LinkUrl { get; set; }
    }

    public partial class SiteserverInnerLink
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
