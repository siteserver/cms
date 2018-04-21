using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongCloudStorage
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("storageID")]
        public long StorageId { get; set; }

        [JsonProperty("providerType")]
        public string ProviderType { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }

    public partial class BairongCloudStorage
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
