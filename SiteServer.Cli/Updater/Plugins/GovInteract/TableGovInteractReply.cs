using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Plugins.GovInteract
{
    public partial class TableGovInteractReply
    {
        [JsonProperty("replyID")]
        public long ReplyId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("contentID")]
        public object ContentId { get; set; }

        [JsonProperty("reply")]
        public string Reply { get; set; }

        [JsonProperty("fileUrl")]
        public string FileUrl { get; set; }

        [JsonProperty("departmentID")]
        public long DepartmentId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }
    }

    public partial class TableGovInteractReply
    {
        public const string OldTableName = "GovInteractReply";

        public static readonly string NewTableName = "ss_govinteract_reply";

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
                ColumnName = "Reply",
                DataType = DataType.Text
            },
            new TableColumnInfo
            {
                ColumnName = "FileUrl",
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
                {"Id", nameof(ReplyId)},
                {"SiteId", nameof(PublishmentSystemId)},
                {"ChannelId", nameof(NodeId)}
            };
    }
}
