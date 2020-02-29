using System;
using Newtonsoft.Json;
using SS.CMS.Abstractions;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableContentCheck
    {
        private readonly IDatabaseManager _databaseManager;

        public TableContentCheck(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("checkID")]
        public long CheckId { get; set; }

        [JsonProperty("tableName")]
        public string TableName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("contentID")]
        public long ContentId { get; set; }

        [JsonProperty("isAdmin")]
        public string IsAdmin { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("checkedLevel")]
        public long CheckedLevel { get; set; }

        [JsonProperty("checkDate")]
        public DateTimeOffset CheckDate { get; set; }

        [JsonProperty("reasons")]
        public string Reasons { get; set; }
    }
}
