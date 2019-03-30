using System;
using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Tables.GovPublic
{
    public partial class TableGovPublicCategory
    {
        [JsonProperty("categoryID")]
        public long CategoryId { get; set; }

        [JsonProperty("classCode")]
        public string ClassCode { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }

        [JsonProperty("categoryCode")]
        public string CategoryCode { get; set; }

        [JsonProperty("parentID")]
        public long ParentId { get; set; }

        [JsonProperty("parentsPath")]
        public string ParentsPath { get; set; }

        [JsonProperty("parentsCount")]
        public long ParentsCount { get; set; }

        [JsonProperty("childrenCount")]
        public long ChildrenCount { get; set; }

        [JsonProperty("isLastNode")]
        public string IsLastNode { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }

        [JsonProperty("contentNum")]
        public long ContentNum { get; set; }
    }

    public partial class TableGovPublicCategory
    {
        public const string OldTableName = "wcm_GovPublicCategory";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = "ss_govpublic_category";

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
                AttributeName = "ClassCode",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "CategoryName",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "CategoryCode",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "ParentId",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "ParentsPath",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "ParentsCount",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "ChildrenCount",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "IsLastNode",
                DataType = DataType.Boolean
            },
            new DatoryColumn
            {
                AttributeName = "Taxis",
                DataType = DataType.Integer
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
            },
            new DatoryColumn
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
