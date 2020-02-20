using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.CMS.Framework;
using SiteServer.CMS.Repositories;


namespace SiteServer.Cli.Updater.Tables
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
