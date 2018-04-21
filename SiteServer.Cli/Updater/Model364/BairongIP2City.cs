using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongIp2City
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("startNum")]
        public long StartNum { get; set; }

        [JsonProperty("endNum")]
        public long EndNum { get; set; }

        [JsonProperty("province")]
        public string Province { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }
    }

    public partial class BairongIp2City
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
