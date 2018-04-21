using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model364
{
    public partial class BairongSmsMessages
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("mobilesList")]
        public string MobilesList { get; set; }

        [JsonProperty("smsContent")]
        public string SmsContent { get; set; }

        [JsonProperty("sendDate")]
        public DateTimeOffset SendDate { get; set; }

        [JsonProperty("smsUserName")]
        public string SmsUserName { get; set; }
    }

    public partial class BairongSmsMessages
    {
        public static readonly string NewTableName = null;

        public static readonly List<TableColumnInfo> NewColumns = null;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
