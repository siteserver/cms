using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model41
{
    public partial class TableTableMatch
    {
        [JsonProperty("tableMatchID")]
        public long TableMatchId { get; set; }

        [JsonProperty("connectionString")]
        public string ConnectionString { get; set; }

        [JsonProperty("tableName")]
        public string TableName { get; set; }

        [JsonProperty("connectionStringToMatch")]
        public string ConnectionStringToMatch { get; set; }

        [JsonProperty("tableNameToMatch")]
        public string TableNameToMatch { get; set; }

        [JsonProperty("columnsMap")]
        public string ColumnsMap { get; set; }
    }

    public partial class TableTableMatch
    {
        public const string OldTableName = "bairong_TableMatch";

        public static readonly string NewTableName = DataProvider.TableMatchDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.TableMatchDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(TableMatchInfo.Id), nameof(TableMatchId)}
            };
    }
}
