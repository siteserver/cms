using Newtonsoft.Json;
using SSCMS;
using SSCMS.Services;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableContentTag
    {
        private readonly IDatabaseManager _databaseManager;

        public TableContentTag(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("tagID")]
        public long TagId { get; set; }

        [JsonProperty("productID")]
        public string ProductId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("contentIDCollection")]
        public string ContentIdCollection { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("useNum")]
        public long UseNum { get; set; }
    }
}
