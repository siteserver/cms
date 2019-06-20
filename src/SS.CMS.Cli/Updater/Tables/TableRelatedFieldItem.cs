using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Updater.Tables
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

        public static ConvertInfo GetConverter(IRelatedFieldItemRepository relatedFieldItemRepository)
        {
            return new ConvertInfo
            {
                NewTableName = relatedFieldItemRepository.TableName,
                NewColumns = relatedFieldItemRepository.TableColumns,
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            }; ;
        }

        private static readonly Dictionary<string, string> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
