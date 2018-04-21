using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverContentGroup
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

    public partial class SiteserverContentGroup
    {
        public static readonly string NewTableName = DataProvider.ContentGroupDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.ContentGroupDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(ContentGroupInfo.GroupName), nameof(ContentGroupName)},
                {nameof(ContentGroupInfo.SiteId), nameof(PublishmentSystemId)}
            };
    }
}
