using Newtonsoft.Json;

namespace SSCMS.Cli.Updater.Tables.GovPublic
{
    public partial class TableGovPublicIdentifierSeq
    {
        [JsonProperty("seqID")]
        public long SeqId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("addYear")]
        public long AddYear { get; set; }

        [JsonProperty("sequence")]
        public long Sequence { get; set; }
    }
}
