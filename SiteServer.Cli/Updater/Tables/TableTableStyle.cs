using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Plugin;

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
            ConvertKeyDict = ConvertKeyDict
        };

        private static readonly string NewTableName = DataProvider.TableStyle.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.TableStyle.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TableStyleInfo.Id), nameof(TableStyleId)},
                {nameof(TableStyleInfo.TableName), nameof(TableName)}
            };
    }
}
