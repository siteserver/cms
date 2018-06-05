using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model41
{
    public partial class TableDbCache
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("cacheKey")]
        public string CacheKey { get; set; }

        [JsonProperty("cacheValue")]
        public string CacheValue { get; set; }
    }

    public partial class TableDbCache
    {
        public const string OldTableName = "Cache";

        public static readonly string NewTableName = DataProvider.DbCacheDao.TableName;

        public static readonly List<TableColumn> NewColumns = DataProvider.DbCacheDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertKeyDict = null;

        public static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
