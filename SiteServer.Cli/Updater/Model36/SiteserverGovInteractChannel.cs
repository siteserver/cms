using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverGovInteractChannel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("applyStyleID")]
        public long ApplyStyleId { get; set; }

        [JsonProperty("queryStyleID")]
        public long QueryStyleId { get; set; }

        [JsonProperty("departmentIDCollection")]
        public string DepartmentIdCollection { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }
    }

    public partial class SiteserverGovInteractChannel
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
