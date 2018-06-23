using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.Cli.Core;
using SiteServer.Cli.Updater.Plugins.GovInteract;
using SiteServer.Cli.Updater.Plugins.GovPublic;
using SiteServer.Cli.Updater.Plugins.Jobs;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

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
        public const string OldTableName = "TableStyle";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.TableStyleDao.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.TableStyleDao.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TableStyleInfo.Id), nameof(TableStyleId)},
                {nameof(TableStyleInfo.TableName), nameof(TableName)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict =
            new Dictionary<string, string>
            {
                {UpdateUtils.GetConvertValueDictKey(nameof(TableStyleInfo.TableName), TableGovInteractContent.OldTableName), TableGovInteractContent.NewTableName},
                {UpdateUtils.GetConvertValueDictKey(nameof(TableStyleInfo.TableName), TableGovPublicContent.OldTableName), TableGovPublicContent.NewTableName},
                {UpdateUtils.GetConvertValueDictKey(nameof(TableStyleInfo.TableName), TableJobsContent.OldTableName), TableJobsContent.NewTableName}
            };
    }
}
