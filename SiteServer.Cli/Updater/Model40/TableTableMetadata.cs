using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model40
{
    public partial class TableTableMetadata
    {
        [JsonProperty("tableMetadataID")]
        public long TableMetadataId { get; set; }

        [JsonProperty("auxiliaryTableENName")]
        public string AuxiliaryTableEnName { get; set; }

        [JsonProperty("attributeName")]
        public string AttributeName { get; set; }

        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonProperty("dataLength")]
        public long DataLength { get; set; }

        [JsonProperty("canBeNull")]
        public string CanBeNull { get; set; }

        [JsonProperty("dbDefaultValue")]
        public string DbDefaultValue { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("isSystem")]
        public string IsSystem { get; set; }
    }

    public partial class TableTableMetadata
    {
        public const string OldTableName = "bairong_TableMetadata";

        public static readonly string NewTableName = DataProvider.TableMetadataDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.TableMetadataDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(TableMetadataInfo.Id), nameof(TableMetadataId)},
                {nameof(TableMetadataInfo.TableName), nameof(AuxiliaryTableEnName)}
            };
    }
}
