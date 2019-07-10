using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Models;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Updater.Tables
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
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_TemplateLog",
            "wcm_TemplateLog"
        };

        public static ConvertInfo GetConverter(ITemplateLogRepository templateLogRepository) => new ConvertInfo
        {
            NewTableName = templateLogRepository.TableName,
            NewColumns = templateLogRepository.TableColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TemplateLog.SiteId), nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
