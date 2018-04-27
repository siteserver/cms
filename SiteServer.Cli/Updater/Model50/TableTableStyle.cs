using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableTableStyle
    {
        [JsonProperty("tableStyleId")]
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
        public bool IsVisible { get; set; }

        [JsonProperty("isVisibleInList")]
        public bool IsVisibleInList { get; set; }

        [JsonProperty("isSingleLine")]
        public bool IsSingleLine { get; set; }

        [JsonProperty("inputType")]
        public string InputType { get; set; }

        [JsonProperty("isRequired")]
        public bool IsRequired { get; set; }

        [JsonProperty("defaultValue")]
        public string DefaultValue { get; set; }

        [JsonProperty("isHorizontal")]
        public bool IsHorizontal { get; set; }

        [JsonProperty("extendValues")]
        public string ExtendValues { get; set; }
    }

    public partial class TableTableStyle
    {
        public const string OldTableName = "bairong_TableStyle";

        public static readonly string NewTableName = DataProvider.TableStyleDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.TableStyleDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(TableStyleInfo.Id), nameof(TableStyleId)}
            };
    }
}
