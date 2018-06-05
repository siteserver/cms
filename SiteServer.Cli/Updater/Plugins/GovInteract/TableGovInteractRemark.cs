using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Plugins.GovInteract
{
    public partial class TableGovInteractRemark
    {
        [JsonProperty("remarkID")]
        public long RemarkId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("contentID")]
        public object ContentId { get; set; }

        [JsonProperty("remarkType")]
        public string RemarkType { get; set; }

        [JsonProperty("remark")]
        public string Remark { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }
    }

    public partial class TableGovInteractRemark
    {
        public const string OldTableName = "GovInteractRemark";

        public static readonly string NewTableName = "ss_govinteract_remark";

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
                ColumnName = "ContentId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "RemarkType",
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = "Remark",
                DataType = DataType.VarChar,
                Length = 255
            },
            new TableColumnInfo
            {
                ColumnName = "DepartmentId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "UserName",
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = "AddDate",
                DataType = DataType.DateTime
            }
        };

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {"Id", nameof(RemarkId)},
                {"SiteId", nameof(PublishmentSystemId)},
                {"ChannelId", nameof(NodeId)}
            };
    }
}
