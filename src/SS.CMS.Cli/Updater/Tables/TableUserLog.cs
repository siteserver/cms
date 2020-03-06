using System;
using Newtonsoft.Json;
using SS.CMS.Abstractions;

namespace SS.CMS.Cli.Updater.Tables
{
    public partial class TableUserLog
    {
        private readonly IDatabaseManager _databaseManager;

        public TableUserLog(IDatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
        }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("ipaddress")]
        public string IpAddress { get; set; }

        [JsonProperty("adddate")]
        public DateTimeOffset Adddate { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("summary")]
        public long Summary { get; set; }
    }
}
