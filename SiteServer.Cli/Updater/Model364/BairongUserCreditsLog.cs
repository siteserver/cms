using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongUserCreditsLog
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("productID")]
        public string ProductId { get; set; }

        [JsonProperty("isIncreased")]
        public string IsIncreased { get; set; }

        [JsonProperty("num")]
        public long Num { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }
    }

    public partial class BairongUserCreditsLog
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
