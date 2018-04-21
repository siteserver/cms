using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongLocalStorage
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("storageID")]
        public long StorageId { get; set; }

        [JsonProperty("directoryPath")]
        public string DirectoryPath { get; set; }
    }

    public partial class BairongLocalStorage
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
