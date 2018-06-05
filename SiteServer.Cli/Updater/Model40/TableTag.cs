using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model40
{
    public partial class TableTag
    {
        [JsonProperty("tagID")]
        public long TagId { get; set; }

        [JsonProperty("productID")]
        public string ProductId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("contentIDCollection")]
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

        public static readonly List<TableColumn> NewColumns = DataProvider.TagDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TagInfo.Id), nameof(TagId)},
                {nameof(TagInfo.SiteId), nameof(PublishmentSystemId)}
            };

        public static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
