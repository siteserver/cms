using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class BairongFtpStorage
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("storageID")]
        public long StorageId { get; set; }

        [JsonProperty("server")]
        public string Server { get; set; }

        [JsonProperty("port")]
        public long Port { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("password")]
        public string Password { get; set; }

        [JsonProperty("isPassiveMode")]
        public string IsPassiveMode { get; set; }
    }

    public partial class BairongFtpStorage
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
