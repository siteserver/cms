using System;
using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;

namespace SS.CMS.Cli.Updater.Tables.GovInteract
{
    public partial class TableGovInteractRemark
    {
        [JsonProperty("remarkID")]
        public long RemarkId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("contentID")]
        public object ContentId { get; set; }

        [JsonProperty("remarkType")]
        public string RemarkType { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }
    }
}
