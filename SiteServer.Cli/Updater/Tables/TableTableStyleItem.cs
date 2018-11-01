using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Tables
{
    public partial class TableTableStyleItem
    {
        [JsonProperty("tableStyleItemID")]
        public long TableStyleItemId { get; set; }

        [JsonProperty("tableStyleID")]
        public long TableStyleId { get; set; }

        [JsonProperty("itemTitle")]
        public string ItemTitle { get; set; }

        [JsonProperty("itemValue")]
        public string ItemValue { get; set; }

        [JsonProperty("isSelected")]
        public string IsSelected { get; set; }
    }

    public partial class TableTableStyleItem
    {
        public const string OldTableName = "bairong_TableStyleItem";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.TableStyleItemDao.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.TableStyleItemDao.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TableStyleItemInfo.Id), nameof(TableStyleItemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
