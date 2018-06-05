using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Plugins.GovPublic
{
    public partial class TableGovPublicIdentifierRule
    {
        [JsonProperty("ruleID")]
        public long RuleId { get; set; }

        [JsonProperty("ruleName")]
        public string RuleName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("identifierType")]
        public string IdentifierType { get; set; }

        [JsonProperty("minLength")]
        public long MinLength { get; set; }

        [JsonProperty("suffix")]
        public string Suffix { get; set; }

        [JsonProperty("formatString")]
        public string FormatString { get; set; }

        [JsonProperty("attributeName")]
        public string AttributeName { get; set; }

        [JsonProperty("sequence")]
        public long Sequence { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }

    public partial class TableGovPublicIdentifierRule
    {
        public const string OldTableName = "GovPublicIdentifierRule";

        public static readonly string NewTableName = "ss_govpublic_identifier_rule";

        public static readonly List<TableColumnInfo> NewColumns = new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = "Id",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "SiteId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "RuleName",
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = "IdentifierType",
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = "MinLength",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "Suffix",
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = "FormatString",
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = "AttributeName",
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = "Sequence",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "Taxis",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "IsSequenceChannelZero",
                DataType = DataType.Boolean
            },
            new TableColumnInfo
            {
                ColumnName = "IsSequenceDepartmentZero",
                DataType = DataType.Boolean
            },
            new TableColumnInfo
            {
                ColumnName = "IsSequenceYearZero",
                DataType = DataType.Boolean
            }
        };

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {"Id", nameof(RuleId)},
                {"SiteId", nameof(PublishmentSystemId)}
            };
    }
}
