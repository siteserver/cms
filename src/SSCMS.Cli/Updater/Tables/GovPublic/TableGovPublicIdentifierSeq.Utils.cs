using System.Collections.Generic;
using Datory;

namespace SSCMS.Cli.Updater.Tables.GovPublic
{
    public partial class TableGovPublicIdentifierSeq
    {
        public const string OldTableName = "wcm_GovPublicIdentifierSeq";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = "ss_govpublic_identifier_seq";

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
                AttributeName = "DepartmentId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "AddYear",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "Sequence",
                DataType = DataType.Integer
            }
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {"Id", nameof(SeqId)},
                {"SiteId", nameof(PublishmentSystemId)},
                {"ChannelId", nameof(NodeId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
