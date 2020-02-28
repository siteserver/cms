using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;

namespace SS.CMS.Cli.Updater.Tables.GovInteract
{
    public partial class TableGovInteractType
    {
        [JsonProperty("typeID")]
        public long TypeId { get; set; }

        [JsonProperty("typeName")]
        public string TypeName { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }
    }
}
