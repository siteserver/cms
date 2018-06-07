using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class TableCount
    {
        [JsonProperty("countID")]
        public long CountId { get; set; }

        [JsonProperty("applicationName")]
        public string ApplicationName { get; set; }

        [JsonProperty("relatedTableName")]
        public string RelatedTableName { get; set; }

        [JsonProperty("relatedIdentity")]
        public string RelatedIdentity { get; set; }

        [JsonProperty("countType")]
        public string CountType { get; set; }

        [JsonProperty("countNum")]
        public long CountNum { get; set; }
    }

    public partial class TableCount
    {
        public const string OldTableName = "Count";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.CountDao.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.CountDao.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(CountInfo.Id), nameof(CountId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
