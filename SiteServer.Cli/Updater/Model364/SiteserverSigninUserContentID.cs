using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverSigninUserContentId
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("isGroup")]
        public string IsGroup { get; set; }

        [JsonProperty("groupID")]
        public long GroupId { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("contentIDCollection")]
        public string ContentIdCollection { get; set; }
    }

    public partial class SiteserverSigninUserContentId
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
