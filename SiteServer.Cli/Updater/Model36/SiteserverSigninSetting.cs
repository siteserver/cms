using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model36
{
    public partial class SiteserverSigninSetting
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("nodeID")]
        public long NodeId { get; set; }

        [JsonProperty("contentID")]
        public long ContentId { get; set; }

        [JsonProperty("isGroup")]
        public string IsGroup { get; set; }

        [JsonProperty("userGroupCollection")]
        public string UserGroupCollection { get; set; }

        [JsonProperty("userNameCollection")]
        public string UserNameCollection { get; set; }

        [JsonProperty("priority")]
        public long Priority { get; set; }

        [JsonProperty("endDate")]
        public string EndDate { get; set; }

        [JsonProperty("isSignin")]
        public string IsSignin { get; set; }

        [JsonProperty("signinDate")]
        public DateTimeOffset SigninDate { get; set; }
    }

    public partial class SiteserverSigninSetting
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
