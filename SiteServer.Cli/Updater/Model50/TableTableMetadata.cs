using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableTableMetadata
    {
        [JsonProperty("tableMetadataId")]
        public long TableMetadataId { get; set; }

        [JsonProperty("auxiliaryTableEnName")]
        public string AuxiliaryTableEnName { get; set; }

        [JsonProperty("attributeName")]
        public string AttributeName { get; set; }

        [JsonProperty("dataType")]
        public string DataType { get; set; }

        [JsonProperty("dataLength")]
        public long DataLength { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("isSystem")]
        public bool IsSystem { get; set; }
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
