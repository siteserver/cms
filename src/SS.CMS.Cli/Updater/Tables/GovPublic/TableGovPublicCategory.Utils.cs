using System.Collections.Generic;
using Datory;

namespace SS.CMS.Cli.Updater.Tables.GovPublic
{
    public partial class TableGovPublicCategory
    {
        private const string NewTableName = "ss_govpublic_category";
        public const string OldTableName = "wcm_GovPublicCategory";

        public static ConvertInfo Converter => new ConvertInfo
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
                AttributeName = "SiteId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "ClassCode",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "CategoryName",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "CategoryCode",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "ParentId",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "ParentsPath",
                DataType = DataType.VarChar,
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "ParentsCount",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "ChildrenCount",
                DataType = DataType.Integer
            },
            new TableColumn
            {
                AttributeName = "LastNode",
                DataType = DataType.Boolean
            },
            new TableColumn
            {
                AttributeName = "Taxis",
                DataType = DataType.Integer
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
                DataLength = 200
            },
            new TableColumn
            {
                AttributeName = "ContentNum",
                DataType = DataType.Integer
            }
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {"Id", nameof(CategoryId)},
                {"SiteId", nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
