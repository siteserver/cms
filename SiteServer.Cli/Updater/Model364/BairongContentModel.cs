using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongContentModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("modelID")]
        public string ModelId { get; set; }

        [JsonProperty("productID")]
        public string ProductId { get; set; }

        [JsonProperty("siteID")]
        public long SiteId { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("isSystem")]
        public string IsSystem { get; set; }

        [JsonProperty("tableName")]
        public string TableName { get; set; }

        [JsonProperty("tableType")]
        public string TableType { get; set; }

        [JsonProperty("iconUrl")]
        public string IconUrl { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class BairongContentModel
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
