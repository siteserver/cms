using Newtonsoft.Json;
using SS.CMS.Abstractions;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableRelatedFieldItem
    {
        private readonly IDatabaseManager _databaseManager;

        public TableRelatedFieldItem(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("relatedFieldID")]
        public long RelatedFieldId { get; set; }

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("itemValue")]
        public string ItemValue { get; set; }

        [JsonProperty("parentID")]
        public long ParentId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }
    }
}
