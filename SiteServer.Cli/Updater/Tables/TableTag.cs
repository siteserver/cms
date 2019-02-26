using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Database.Core;
using SiteServer.CMS.Database.Models;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Tables
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
        public const string OldTableName = "bairong_Tags";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.Tag.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.Tag.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TagInfo.Id), nameof(TagId)},
                {nameof(TagInfo.SiteId), nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
