using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Models;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Updater.Tables
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

        public static ConvertInfo GetConverter(ITableStyleItemRepository tableStyleItemRepository) => new ConvertInfo
        {
            NewTableName = tableStyleItemRepository.TableName,
            NewColumns = tableStyleItemRepository.TableColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TableStyleItem.Id), nameof(TableStyleItemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
