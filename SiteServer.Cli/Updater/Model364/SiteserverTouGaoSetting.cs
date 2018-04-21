using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class SiteserverTouGaoSetting
    {
        [JsonProperty("settingID")]
        public long SettingId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("userTypeID")]
        public long UserTypeId { get; set; }

        [JsonProperty("isTouGaoAllowed")]
        public string IsTouGaoAllowed { get; set; }

        [JsonProperty("checkLevel")]
        public long CheckLevel { get; set; }

        [JsonProperty("isCheckAllowed")]
        public string IsCheckAllowed { get; set; }

        [JsonProperty("isCheckAddedUsersOnly")]
        public string IsCheckAddedUsersOnly { get; set; }

        [JsonProperty("checkUserTypeIDCollection")]
        public string CheckUserTypeIdCollection { get; set; }
    }

    public partial class SiteserverTouGaoSetting
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
