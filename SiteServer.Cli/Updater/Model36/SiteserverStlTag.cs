using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverStlTag
    {
        [JsonProperty("tagName")]
        public string TagName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("tagDescription")]
        public string TagDescription { get; set; }

        [JsonProperty("tagContent")]
        public string TagContent { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class SiteserverStlTag
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
