using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Models;
using SS.CMS.Repositories;
using SS.CMS.Utils;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableTableStyle
    {
        [JsonProperty("tableStyleID")]
        public long TableStyleId { get; set; }

        [JsonProperty("relatedIdentity")]
        public long RelatedIdentity { get; set; }

        [JsonProperty("tableName")]
        public string TableName { get; set; }

        [JsonProperty("attributeName")]
        public string AttributeName { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("helpText")]
        public string HelpText { get; set; }

        [JsonProperty("isVisible")]
        public string IsVisible { get; set; }

        [JsonProperty("isVisibleInList")]
        public string IsVisibleInList { get; set; }

        [JsonProperty("isSingleLine")]
        public string IsSingleLine { get; set; }

        [JsonProperty("inputType")]
        public string InputType { get; set; }

        [JsonProperty("isRequired")]
        public string IsRequired { get; set; }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("isHorizontal")]
        public string IsHorizontal { get; set; }

        [JsonProperty("extendValues")]
        public string ExtendValues { get; set; }
    }

    public partial class TableTableStyle
    {
        public const string OldTableName = "bairong_TableStyle";

        public static ConvertInfo GetConverter(ITableStyleRepository tableStyleRepository, ISiteRepository siteRepository, IChannelRepository channelRepository) => new ConvertInfo
        {
            NewTableName = tableStyleRepository.TableName,
            NewColumns = tableStyleRepository.TableColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict(siteRepository, channelRepository),
            Process = Process
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TableStyleInfo.Id), nameof(TableStyleId)},
                {nameof(TableStyleInfo.TableName), nameof(TableName)}
            };

        private static Dictionary<string, string> ConvertValueDict(ISiteRepository siteRepository, IChannelRepository channelRepository) => new Dictionary<string, string>
        {
            {UpdateUtils.GetConvertValueDictKey(nameof(TableStyleInfo.TableName), "siteserver_PublishmentSystem"), siteRepository.TableName
            },
            {UpdateUtils.GetConvertValueDictKey(nameof(TableStyleInfo.TableName), "siteserver_Node"), channelRepository.TableName
            }
        };

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue("IsVisible", out var isVisible))
            {
                if (isVisible != null && StringUtils.EqualsIgnoreCase(isVisible.ToString(), "False"))
                {
                    row["InputType"] = SS.CMS.Enums.InputType.Hidden.Value;
                }
            }

            return row;
        }
    }
}
