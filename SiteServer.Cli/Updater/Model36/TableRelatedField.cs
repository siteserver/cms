using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class TableRelatedField
    {
        [JsonProperty("relatedFieldID")]
        public long RelatedFieldId { get; set; }

        [JsonProperty("relatedFieldName")]
        public string RelatedFieldName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("totalLevel")]
        public long TotalLevel { get; set; }

        [JsonProperty("prefixes")]
        public string Prefixes { get; set; }

        [JsonProperty("suffixes")]
        public string Suffixes { get; set; }
    }

    public partial class TableRelatedField
    {
        public const string OldTableName = "siteserver_RelatedField";

        public static readonly string NewTableName = DataProvider.RelatedFieldDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.RelatedFieldDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(RelatedFieldInfo.Id), nameof(RelatedFieldId)},
                {nameof(RelatedFieldInfo.Title), nameof(RelatedFieldName)},
                {nameof(RelatedFieldInfo.SiteId), nameof(PublishmentSystemId)}
            };
    }
}
