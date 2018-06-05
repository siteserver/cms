using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Plugins.GovInteract
{
    public partial class TableGovInteractChannel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("applyStyleID")]
        public long ApplyStyleId { get; set; }

        [JsonProperty("queryStyleID")]
        public long QueryStyleId { get; set; }

        [JsonProperty("departmentIDCollection")]
        public string DepartmentIdCollection { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }
    }

    public partial class TableGovInteractChannel
    {
        public const string OldTableName = "GovInteractChannel";

        public static readonly string NewTableName = "ss_govinteract_channel";

        public static readonly List<TableColumnInfo> NewColumns = new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = "ChannelId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "SiteId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "ApplyStyleId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "QueryStyleId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "DepartmentIdCollection",
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = "Summary",
                DataType = DataType.VarChar,
                Length = 255
            }
        };

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {"ChannelId", nameof(NodeId)},
                {"SiteId", nameof(PublishmentSystemId)}
            };
    }
}
