using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Models;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Updater.Tables
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

        [JsonProperty("systemConfig")]
        public string SystemConfig { get; set; }
    }

    public partial class TableConfig
    {
        public const string OldTableName = "bairong_Config";

        public static ConvertInfo GetConverter(IConfigRepository configRepository)
        {
            return new ConvertInfo
            {
                NewTableName = configRepository.TableName,
                NewColumns = configRepository.TableColumns,
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            };
        }

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(ConfigInfo.ExtendValues), nameof(SystemConfig)},
                {nameof(ConfigInfo.ExtendValues), nameof(SettingsXml)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
