using Newtonsoft.Json;
using SS.CMS.Abstractions;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TablePermissionsInRoles
    {
        private readonly IDatabaseManager _databaseManager;

        public TablePermissionsInRoles(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("generalPermissions")]
        public string GeneralPermissions { get; set; }
    }
}
