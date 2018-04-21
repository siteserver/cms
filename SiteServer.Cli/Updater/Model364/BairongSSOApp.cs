using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongSsoApp
    {
        [JsonProperty("appID")]
        public long AppId { get; set; }

        [JsonProperty("appType")]
        public string AppType { get; set; }

        [JsonProperty("appName")]
        public string AppName { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("authKey")]
        public string AuthKey { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("accessFileName")]
        public string AccessFileName { get; set; }

        [JsonProperty("isSyncLogin")]
        public string IsSyncLogin { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }
    }

    public partial class BairongSsoApp
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
