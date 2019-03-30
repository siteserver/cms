using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Tables.GovPublic
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
        public const string OldTableName = "wcm_GovPublicIdentifierRule";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = "ss_govpublic_identifier_rule";

        private static readonly List<DatoryColumn> NewColumns = new List<DatoryColumn>
        {
            new DatoryColumn
            {
                AttributeName = "Id",
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new DatoryColumn
            {
                AttributeName = "SiteId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "RuleName",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "IdentifierType",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "MinLength",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "Suffix",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "FormatString",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "AttributeName",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "Sequence",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "Taxis",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "IsSequenceChannelZero",
                DataType = DataType.Boolean
            },
            new DatoryColumn
            {
                AttributeName = "IsSequenceDepartmentZero",
                DataType = DataType.Boolean
            },
            new DatoryColumn
            {
                AttributeName = "IsSequenceYearZero",
                DataType = DataType.Boolean
            }
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {"Id", nameof(RuleId)},
                {"SiteId", nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
