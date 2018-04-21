using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverUserGroup
    {
        [JsonProperty("groupID")]
        public long GroupId { get; set; }

        [JsonProperty("groupName")]
        public string GroupName { get; set; }

        [JsonProperty("isCredits")]
        public string IsCredits { get; set; }

        [JsonProperty("creditsFrom")]
        public long CreditsFrom { get; set; }

        [JsonProperty("creditsTo")]
        public long CreditsTo { get; set; }

        [JsonProperty("stars")]
        public long Stars { get; set; }

        [JsonProperty("color")]
        public string Color { get; set; }

        [JsonProperty("rank")]
        public long Rank { get; set; }

        [JsonProperty("userPermissions")]
        public string UserPermissions { get; set; }
    }

    public partial class SiteserverUserGroup
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
