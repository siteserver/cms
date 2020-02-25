using System;
using System.Collections.Generic;
using Datory;
using Newtonsoft.Json;
using SS.CMS.Framework;

namespace SS.CMS.Cli.Updater.Tables
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

        public static ConvertInfo Converter => new ConvertInfo
        {
            NewTableName = NewTableName,
            NewColumns = NewColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly string NewTableName = DataProvider.UserLogRepository.TableName;

        private static readonly List<TableColumn> NewColumns = DataProvider.UserLogRepository.TableColumns;

        private static readonly Dictionary<string, string> ConvertKeyDict = null;

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
