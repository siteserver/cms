using Newtonsoft.Json;
using SSCMS;
using SSCMS.Services;

namespace SSCMS.Cli.Updater.Tables
{
    public partial class TableRelatedField
    {
        private readonly IDatabaseManager _databaseManager;

        public TableRelatedField(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("relatedFieldID")]
        public long RelatedFieldId { get; set; }

        [JsonProperty("relatedFieldName")]
        public string RelatedFieldName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("totalLevel")]
        public long TotalLevel { get; set; }

        [JsonProperty("prefixes")]
        public string Prefixes { get; set; }

        [JsonProperty("suffixes")]
        public string Suffixes { get; set; }
    }
}
