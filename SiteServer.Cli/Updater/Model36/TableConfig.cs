using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class TableConfig
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("isInitialized")]
        public string IsInitialized { get; set; }

        [JsonProperty("databaseVersion")]
        public string DatabaseVersion { get; set; }

        [JsonProperty("restrictionBlackList")]
        public string RestrictionBlackList { get; set; }

        [JsonProperty("restrictionWhiteList")]
        public string RestrictionWhiteList { get; set; }

        [JsonProperty("isRelatedUrl")]
        public string IsRelatedUrl { get; set; }

        [JsonProperty("rootUrl")]
        public string RootUrl { get; set; }

        [JsonProperty("updateDate")]
        public DateTimeOffset UpdateDate { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }

    public partial class TableConfig
    {
        public const string OldTableName = "bairong_Config";

        public static readonly string NewTableName = DataProvider.ConfigDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.ConfigDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(ConfigInfo.SystemConfig), nameof(SettingsXml)}
            };
    }
}
