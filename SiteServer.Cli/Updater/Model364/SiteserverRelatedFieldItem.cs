using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverRelatedFieldItem
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

    public partial class SiteserverRelatedFieldItem
    {
        public static readonly string NewTableName = DataProvider.RelatedFieldItemDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.RelatedFieldItemDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(RelatedFieldItemInfo.Id), nameof(RelatedFieldId)}
            };
    }
}
