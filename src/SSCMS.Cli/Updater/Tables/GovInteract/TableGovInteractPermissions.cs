using Newtonsoft.Json;

namespace SSCMS.Cli.Updater.Tables.GovInteract
{
    public partial class TableGovInteractPermissions
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("permissions")]
        public string Permissions { get; set; }
    }
}
