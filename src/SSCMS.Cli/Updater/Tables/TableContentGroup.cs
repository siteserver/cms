using Newtonsoft.Json;
using SSCMS;
using SSCMS.Services;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableContentGroup
    {
        private readonly IDatabaseManager _databaseManager;

        public TableContentGroup(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("contentGroupName")]
        public string ContentGroupName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }
}
