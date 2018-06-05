using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableTag
    {
        [JsonProperty("tagId")]
        public long TagId { get; set; }

        [JsonProperty("publishmentSystemId")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("contentIdCollection")]
        public string ContentIdCollection { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("useNum")]
        public long UseNum { get; set; }
    }

    public partial class TableTag
    {
        public const string OldTableName = "Tags";

        public static readonly string NewTableName = DataProvider.TagDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.TagDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(TagInfo.Id), nameof(TagId)},
                {nameof(TagInfo.SiteId), nameof(PublishmentSystemId)}
            };
    }
}
