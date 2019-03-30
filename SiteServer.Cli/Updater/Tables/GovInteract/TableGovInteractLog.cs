using System;
using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Tables.GovInteract
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
        public const string OldTableName = "wcm_GovInteractLog";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = "ss_govinteract_log";

        private static readonly List<DatoryColumn> NewColumns = new List<DatoryColumn>
        {
            new DatoryColumn
            {
                AttributeName = "Id",
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new DatoryColumn
            {
                AttributeName = "SiteId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "ChannelId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "ContentId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "DepartmentId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "UserName",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "LogType",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "IpAddress",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "AddDate",
                DataType = DataType.DateTime
            },
            new DatoryColumn
            {
                AttributeName = "Summary",
                DataType = DataType.VarChar
            }
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {"Id", nameof(LogId)},
                {"SiteId", nameof(PublishmentSystemId)},
                {"ChannelId", nameof(NodeId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
