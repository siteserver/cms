using System.Collections.Generic;
using Datory;

namespace SS.CMS.Cli.Updater.Tables.GovPublic
{
    public partial class TableGovPublicIdentifierRule
    {
        public const string OldTableName = "wcm_GovPublicIdentifierRule";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = "ss_govpublic_identifier_rule";

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
                AttributeName = "RuleName",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "IdentifierType",
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = "MinLength",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "Suffix",
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = "FormatString",
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = "AttributeName",
                DataType = DataType.VarChar,
                DataLength = 50
            },
            new TableColumn
            {
                AttributeName = "Sequence",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "Taxis",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "IsSequenceChannelZero",
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = "IsSequenceDepartmentZero",
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = "IsSequenceYearZero",
                DataType = DataType.Boolean
            }
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {"Id", nameof(RuleId)},
                {"SiteId", nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
