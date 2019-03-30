using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Tables.GovPublic
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
        public const string OldTableName = "wcm_GovPublicCategoryClass";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = "ss_govpublic_category_class";

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
                AttributeName = "ClassName",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "IsSystem",
                DataType = DataType.Boolean
            },
            new DatoryColumn
            {
                AttributeName = "IsEnabled",
                DataType = DataType.Boolean
            },
            new DatoryColumn
            {
                AttributeName = "ContentAttributeName",
                DataType = DataType.VarChar
            },
            new DatoryColumn
            {
                AttributeName = "Taxis",
                DataType = DataType.Integer
            },
            new DatoryColumn
            {
                AttributeName = "Description",
                DataType = DataType.VarChar
            }
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {"SiteId", nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
