using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongTask
    {
        [JsonProperty("taskID")]
        public long TaskId { get; set; }

        [JsonProperty("taskName")]
        public string TaskName { get; set; }

        [JsonProperty("productID")]
        public string ProductId { get; set; }

        [JsonProperty("isSystemTask")]
        public string IsSystemTask { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("serviceType")]
        public string ServiceType { get; set; }

        [JsonProperty("serviceParameters")]
        public string ServiceParameters { get; set; }

        [JsonProperty("frequencyType")]
        public string FrequencyType { get; set; }

        [JsonProperty("periodIntervalMinute")]
        public long PeriodIntervalMinute { get; set; }

        [JsonProperty("startDay")]
        public long StartDay { get; set; }

        [JsonProperty("startWeekday")]
        public long StartWeekday { get; set; }

        [JsonProperty("startHour")]
        public long StartHour { get; set; }

        [JsonProperty("isEnabled")]
        public string IsEnabled { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("lastExecuteDate")]
        public DateTimeOffset LastExecuteDate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class BairongTask
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
