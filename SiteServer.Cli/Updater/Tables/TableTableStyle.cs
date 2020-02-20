using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Abstractions;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;

namespace SiteServer.Cli.Updater.Tables
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

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict,
            Process = Process
        };

        private static readonly string NewTableName = DataProvider.TableStyleRepository.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.TableStyleRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TableStyle.Id), nameof(TableStyleId)},
                {nameof(TableStyle.TableName), nameof(TableName)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = new Dictionary<string, string>
        {
            {UpdateUtils.GetConvertValueDictKey(nameof(TableStyle.TableName), "siteserver_PublishmentSystem"), DataProvider.SiteRepository.TableName},
            {UpdateUtils.GetConvertValueDictKey(nameof(TableStyle.TableName), "siteserver_Node"), DataProvider.ChannelRepository.TableName}
        };

        private static Dictionary<string, object> Process(Dictionary<string, object> row)
        {
            if (row.TryGetValue("IsVisible", out var isVisible))
            {
                if (isVisible != null && StringUtils.EqualsIgnoreCase(isVisible.ToString(), "False"))
                {
                    row[nameof(TableStyle.InputType)] = Abstractions.InputType.Hidden.GetValue();
                }
            }

            return row;
        }
    }
}
