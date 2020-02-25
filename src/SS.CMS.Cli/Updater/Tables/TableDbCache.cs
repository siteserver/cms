using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SS.CMS.Framework;

namespace SS.CMS.Cli.Updater.Tables
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
        public const string OldTableName = "bairong_Cache";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.DbCacheRepository.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.DbCacheRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
