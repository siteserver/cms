using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class BairongModule
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("moduleID")]
        public string ModuleId { get; set; }

        [JsonProperty("directoryName")]
        public string DirectoryName { get; set; }

        [JsonProperty("isRoot")]
        public string IsRoot { get; set; }
    }

    public partial class BairongModule
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
