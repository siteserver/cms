using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongTaskLog
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("taskID")]
        public long TaskId { get; set; }

        [JsonProperty("isSuccess")]
        public string IsSuccess { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }

        [JsonProperty("stackTrace")]
        public string StackTrace { get; set; }

        [JsonProperty("subtaskErrors")]
        public string SubtaskErrors { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }
    }

    public partial class BairongTaskLog
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
