using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model41
{
    public partial class TableErrorLog
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("stacktrace")]
        public string Stacktrace { get; set; }

        [JsonProperty("summary")]
        public string Summary { get; set; }
    }

    public partial class TableErrorLog
    {
        public const string OldTableName = "ErrorLog";

        public static readonly string NewTableName = DataProvider.ErrorLogDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.ErrorLogDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict = null;
    }
}
