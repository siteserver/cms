using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableTemplateLog
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("templateId")]
        public long TemplateId { get; set; }

        [JsonProperty("publishmentSystemId")]
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
        public const string OldTableName = "TemplateLog";

        public static readonly string NewTableName = DataProvider.TemplateLogDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.TemplateLogDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(TemplateLogInfo.SiteId), nameof(PublishmentSystemId)}
            };
    }
}
