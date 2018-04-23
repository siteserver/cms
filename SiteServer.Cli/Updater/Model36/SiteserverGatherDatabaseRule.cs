using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverGatherDatabaseRule
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("gatherRuleName")]
        public string GatherRuleName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("databaseType")]
        public string DatabaseType { get; set; }

        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }

        [JsonProperty("relatedTableName")]
        public string RelatedTableName { get; set; }

        [JsonProperty("relatedIdentity")]
        public string RelatedIdentity { get; set; }

        [JsonProperty("relatedOrderBy")]
        public string RelatedOrderBy { get; set; }

        [JsonProperty("whereString")]
        public string WhereString { get; set; }

        [JsonProperty("tableMatchID")]
        public long TableMatchId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("gatherNum")]
        public long GatherNum { get; set; }

        [JsonProperty("isChecked")]
        public string IsChecked { get; set; }

        [JsonProperty("isOrderByDesc")]
        public string IsOrderByDesc { get; set; }

        [JsonProperty("lastGatherDate")]
        public DateTimeOffset LastGatherDate { get; set; }
    }

    public partial class SiteserverGatherDatabaseRule
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
