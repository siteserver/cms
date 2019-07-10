using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Models;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Updater.Tables
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

        public static ConvertInfo GetConverter(IContentGroupRepository contentGroupRepository)
        {
            return new ConvertInfo
            {
                NewTableName = contentGroupRepository.TableName,
                NewColumns = contentGroupRepository.TableColumns,
                ConvertKeyDict = ConvertKeyDict,
                ConvertValueDict = ConvertValueDict
            };
        }

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(ContentGroup.GroupName), nameof(ContentGroupName)},
                {nameof(ContentGroup.SiteId), nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
