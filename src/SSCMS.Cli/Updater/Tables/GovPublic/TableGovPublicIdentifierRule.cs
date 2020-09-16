using Newtonsoft.Json;

namespace SSCMS.Cli.Updater.Tables.GovPublic
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
}
