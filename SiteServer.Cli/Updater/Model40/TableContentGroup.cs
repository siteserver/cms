using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model40
{
    public partial class TableContentGroup
    {
        [JsonProperty("contentGroupName")]
        public string ContentGroupName { get; set; }

        [JsonProperty("publishmentSystemId")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }
    }

    public partial class TableContentGroup
    {
        public const string OldTableName = "ContentGroup";

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.ContentGroupDao.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.ContentGroupDao.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(ContentGroupInfo.GroupName), nameof(ContentGroupName)},
                {nameof(ContentGroupInfo.SiteId), nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
