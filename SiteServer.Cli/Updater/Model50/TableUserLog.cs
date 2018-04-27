using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableUserLog
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("userName")]
        public string UserName { get; set; }

        [JsonProperty("ipaddress")]
        public string IpAddress { get; set; }

        [JsonProperty("adddate")]
        public DateTimeOffset Adddate { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("summary")]
        public long Summary { get; set; }
    }

    public partial class TableUserLog
    {
        public const string OldTableName = "bairong_UserLog";

        public static readonly string NewTableName = DataProvider.UserLogDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.UserLogDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
