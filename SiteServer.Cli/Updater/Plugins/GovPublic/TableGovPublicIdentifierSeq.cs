using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Plugins.GovPublic
{
    public partial class TableGovPublicIdentifierSeq
    {
        [JsonProperty("seqID")]
        public long SeqId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("addYear")]
        public long AddYear { get; set; }

        [JsonProperty("sequence")]
        public long Sequence { get; set; }
    }

    public partial class TableGovPublicIdentifierSeq
    {
        public const string OldTableName = "GovPublicIdentifierSeq";

        public static readonly string NewTableName = "ss_govpublic_identifier_seq";

        public static readonly List<TableColumnInfo> NewColumns = new List<TableColumnInfo>
        {
            new TableColumnInfo
            {
                ColumnName = "Id",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "SiteId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "ChannelId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "DepartmentId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "AddYear",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "Sequence",
                DataType = DataType.Integer
            }
        };

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {"Id", nameof(SeqId)},
                {"SiteId", nameof(PublishmentSystemId)},
                {"ChannelId", nameof(NodeId)}
            };
    }
}
