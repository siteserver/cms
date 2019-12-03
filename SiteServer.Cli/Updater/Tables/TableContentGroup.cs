using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SiteServer.Abstractions;
using SiteServer.CMS.Repositories;


namespace SiteServer.Cli.Updater.Tables
{
    public partial class TableContentGroup
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("contentGroupName")]
        public string ContentGroupName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("taxis")]
        public long Taxis { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public partial class TableContentGroup
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_ContentGroup",
            "wcm_ContentGroup"
        };

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.ContentGroupRepository.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.ContentGroupRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(ContentGroup.GroupName), nameof(ContentGroupName)},
                {nameof(ContentGroup.SiteId), nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
