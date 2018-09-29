using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Tables
{
    public partial class TableRelatedFieldItem
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("relatedFieldID")]
        public long RelatedFieldId { get; set; }

        [JsonProperty("itemName")]
        public string ItemName { get; set; }

        [JsonProperty("itemValue")]
        public string ItemValue { get; set; }

        [JsonProperty("parentID")]
        public long ParentId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }
    }

    public partial class TableRelatedFieldItem
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_RelatedFieldItem",
            "wcm_RelatedFieldItem"
        };

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.RelatedFieldItemDao.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.RelatedFieldItemDao.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
