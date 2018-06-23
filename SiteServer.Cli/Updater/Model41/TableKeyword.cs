using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model41
{
    public partial class TableKeyword
    {
        [JsonProperty("keywordId")]
        public long KeywordId { get; set; }

        [JsonProperty("keyword")]
        public string Keyword { get; set; }

        [JsonProperty("alternative")]
        public string Alternative { get; set; }

        [JsonProperty("grade")]
        public string Grade { get; set; }
    }

    public partial class TableKeyword
    {
        public const string OldTableName = "Keyword";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.KeywordDao.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.KeywordDao.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(KeywordInfo.Id), nameof(KeywordId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
