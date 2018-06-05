using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Plugins.GovPublic
{
    public partial class TableGovPublicCategoryClass
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("classCode")]
        public string ClassCode { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("className")]
        public string ClassName { get; set; }

        [JsonProperty("isSystem")]
        public string IsSystem { get; set; }

        [JsonProperty("isEnabled")]
        public string IsEnabled { get; set; }

        [JsonProperty("contentAttributeName")]
        public string ContentAttributeName { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class TableGovPublicCategoryClass
    {
        public const string OldTableName = "GovPublicCategoryClass";

        public static readonly string NewTableName = "ss_govpublic_category_class";

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
                ColumnName = "ClassName",
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = "IsSystem",
                DataType = DataType.Boolean
            },
            new TableColumnInfo
            {
                ColumnName = "IsEnabled",
                DataType = DataType.Boolean
            },
            new TableColumnInfo
            {
                ColumnName = "ContentAttributeName",
                DataType = DataType.VarChar,
                Length = 200
            },
            new TableColumnInfo
            {
                ColumnName = "Taxis",
                DataType = DataType.Integer
            },
            new TableColumnInfo
            {
                ColumnName = "Description",
                DataType = DataType.VarChar,
                Length = 200
            }
        };

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {"SiteId", nameof(PublishmentSystemId)}
            };
    }
}
