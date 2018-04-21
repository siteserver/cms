using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongUserType
    {
        [JsonProperty("typeID")]
        public long TypeId { get; set; }

        [JsonProperty("typeName")]
        public string TypeName { get; set; }

        [JsonProperty("isDefault")]
        public string IsDefault { get; set; }

        [JsonProperty("isPermissions")]
        public string IsPermissions { get; set; }

        [JsonProperty("permissions")]
        public string Permissions { get; set; }
    }

    public partial class BairongUserType
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
