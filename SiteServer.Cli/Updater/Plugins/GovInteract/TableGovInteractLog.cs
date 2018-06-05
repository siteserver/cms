using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Plugins.GovInteract
{
    public partial class TableGovInteractLog
    {
        [JsonProperty("logID")]
        public long LogId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("contentID")]
        public object ContentId { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("logType")]
        public string LogType { get; set; }

        [JsonProperty("ipAddress")]
        public string IpAddress { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }
    }

    public partial class TableGovInteractLog
    {
        public const string OldTableName = "GovInteractLog";

        public static readonly string NewTableName = "ss_govinteract_log";

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
                ColumnName = "LogType",
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = "IpAddress",
                DataType = DataType.VarChar,
                Length = 50
            },
            new TableColumnInfo
            {
                ColumnName = "AddDate",
                DataType = DataType.DateTime
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
                {"Id", nameof(LogId)},
                {"SiteId", nameof(PublishmentSystemId)},
                {"ChannelId", nameof(NodeId)}
            };
    }
}
