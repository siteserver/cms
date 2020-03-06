using Newtonsoft.Json;

namespace SS.CMS.Cli.Updater.Tables.GovInteract
{
    public partial class TableGovInteractChannel
    {
        public TableGovInteractChannel()
        {

        }

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
}
