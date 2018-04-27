using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model41
{
    public partial class TableTemplateLog
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("templateID")]
        public long TemplateId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("addDate")]
        public DateTimeOffset AddDate { get; set; }

        [JsonProperty("addUserName")]
        public string AddUserName { get; set; }

        [JsonProperty("contentLength")]
        public long ContentLength { get; set; }

        [JsonProperty("templateContent")]
        public string TemplateContent { get; set; }
    }

    public partial class TableTemplateLog
    {
        public const string OldTableName = "siteserver_TemplateLog";

        public static readonly string NewTableName = DataProvider.TemplateLogDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.TemplateLogDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(TemplateLogInfo.SiteId), nameof(PublishmentSystemId)}
            };
    }
}
