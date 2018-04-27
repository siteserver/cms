using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model40
{
    public partial class TableTable
    {
        [JsonProperty("tableENName")]
        public string TableEnName { get; set; }

        [JsonProperty("tableCNName")]
        public string TableCnName { get; set; }

        [JsonProperty("attributeNum")]
        public long AttributeNum { get; set; }

        [JsonProperty("auxiliaryTableType")]
        public string AuxiliaryTableType { get; set; }

        [JsonProperty("isCreatedInDB")]
        public string IsCreatedInDb { get; set; }

        [JsonProperty("isChangedAfterCreatedInDB")]
        public string IsChangedAfterCreatedInDb { get; set; }

        [JsonProperty("isDefault")]
        public string IsDefault { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class TableTable
    {
        public const string OldTableName = "bairong_TableCollection";

        public static readonly string NewTableName = DataProvider.TableDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.TableDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(TableInfo.TableName), nameof(TableEnName)},
                {nameof(TableInfo.DisplayName), nameof(TableCnName)}
            };
    }
}
