using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater.Tables.GovInteract
{
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

        private static readonly List<TableColumn> NewColumns = new List<TableColumn>
        {
            new TableColumn
            {
                AttributeName = "Id",
                DataType = DataType.Integer,
                IsPrimaryKey = true,
                IsIdentity = true
            },
            new TableColumn
            {
                AttributeName = "SiteId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "ChannelId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "ContentId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "DepartmentId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "UserName",
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = "LogType",
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = "IpAddress",
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = "AddDate",
                DataType = DataType.DateTime
            },
            new TableColumn
            {
                AttributeName = "Summary",
                DataType = DataType.VarChar,
                DataLength = 255
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
