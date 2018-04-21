using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverGovPublicIdentifierRule
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

    public partial class SiteserverGovPublicIdentifierRule
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
