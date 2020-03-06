using Newtonsoft.Json;
using SS.CMS.Abstractions;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableRole
    {
        private readonly IDatabaseManager _databaseManager;

        public TableRole(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("roleName")]
        public string RoleName { get; set; }

        [JsonProperty("modules")]
        public string Modules { get; set; }

        [JsonProperty("creatorUserName")]
        public string CreatorUserName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
