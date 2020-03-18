using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater.Tables.GovInteract
{
    public partial class TableGovInteractChannel
    {
        public const string OldTableName = "wcm_GovInteractChannel";
        public const string NewTableName = "ss_govinteract_channel";

        public ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

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
                AttributeName = "ChannelId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "SiteId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "ApplyStyleId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "QueryStyleId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "DepartmentIdCollection",
                DataType = DataType.VarChar,
                DataLength = 255
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
                {"ChannelId", nameof(NodeId)},
                {"SiteId", nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
