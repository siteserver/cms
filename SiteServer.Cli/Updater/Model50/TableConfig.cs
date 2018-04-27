using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableConfig
    {
        [JsonProperty("isInitialized")]
        public string IsInitialized { get; set; }

        [JsonProperty("databaseVersion")]
        public string DatabaseVersion { get; set; }

        [JsonProperty("restrictionBlackList")]
        public string RestrictionBlackList { get; set; }

        [JsonProperty("restrictionWhiteList")]
        public string RestrictionWhiteList { get; set; }

        [JsonProperty("updateDate")]
        public DateTimeOffset UpdateDate { get; set; }

        [JsonProperty("userConfig")]
        public string UserConfig { get; set; }

        [JsonProperty("systemConfig")]
        public string SystemConfig { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class TableConfig
    {
        public const string OldTableName = "bairong_Config";

        public static readonly string NewTableName = DataProvider.ConfigDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.ConfigDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
