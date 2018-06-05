using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Plugins.GovPublic
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
        public const string OldTableName = "GovPublicCategory";

        public static readonly string NewTableName = "ss_govpublic_category";

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
                ColumnName = "ClassCode",
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = "CategoryName",
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = "CategoryCode",
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = "ParentId",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "ParentsPath",
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = "ParentsCount",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "ChildrenCount",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "IsLastNode",
                DataType = DataType.Boolean
            },
            new TableColumnInfo
            {
                ColumnName = "Taxis",
                DataType = DataType.Integer
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
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = "ContentNum",
                DataType = DataType.Integer
            }
        };

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {"Id", nameof(CategoryId)},
                {"SiteId", nameof(PublishmentSystemId)}
            };
    }
}
