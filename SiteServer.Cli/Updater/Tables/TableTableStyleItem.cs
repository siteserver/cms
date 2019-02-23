using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
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

        private static readonly string NewTableName = DataProvider.TableStyleItem.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.TableStyleItem.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TableStyleItemInfo.Id), nameof(TableStyleItemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
