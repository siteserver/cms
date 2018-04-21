using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverUsers
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("groupID")]
        public long GroupId { get; set; }

        [JsonProperty("credits")]
        public long Credits { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }

    public partial class SiteserverUsers
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
