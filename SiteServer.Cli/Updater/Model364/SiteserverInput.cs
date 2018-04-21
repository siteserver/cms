using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverInput
    {
        [JsonProperty("inputID")]
        public long InputId { get; set; }

        [JsonProperty("inputName")]
        public string InputName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("isReply")]
        public string IsReply { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("isTemplate")]
        public string IsTemplate { get; set; }

        [JsonProperty("styleTemplate")]
        public string StyleTemplate { get; set; }

        [JsonProperty("scriptTemplate")]
        public string ScriptTemplate { get; set; }

        [JsonProperty("contentTemplate")]
        public string ContentTemplate { get; set; }

        [JsonProperty("settingsXML")]
        public string SettingsXml { get; set; }
    }

    public partial class SiteserverInput
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
