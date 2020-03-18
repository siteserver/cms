using System;
using Newtonsoft.Json;

namespace SSCMS.Cli.Updater.Tables.GovInteract
{
    public partial class TableGovInteractLog
    {
        [JsonProperty("logID")]
        public long LogId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("contentID")]
        public object ContentId { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("logType")]
        public string LogType { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }
    }
}
