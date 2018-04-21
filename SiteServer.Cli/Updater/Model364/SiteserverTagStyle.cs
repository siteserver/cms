using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverTagStyle
    {
        [JsonProperty("styleID")]
        public long StyleId { get; set; }

        [JsonProperty("styleName")]
        public string StyleName { get; set; }

        [JsonProperty("elementName")]
        public string ElementName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("isTemplate")]
        public string IsTemplate { get; set; }

        [JsonProperty("styleTemplate")]
        public string StyleTemplate { get; set; }

        [JsonProperty("scriptTemplate")]
        public string ScriptTemplate { get; set; }

        [JsonProperty("contentTemplate")]
        public string ContentTemplate { get; set; }

        [JsonProperty("successTemplate")]
        public string SuccessTemplate { get; set; }

        [JsonProperty("failureTemplate")]
        public string FailureTemplate { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }

    public partial class SiteserverTagStyle
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
