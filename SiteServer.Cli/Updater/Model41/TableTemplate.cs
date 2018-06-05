using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;

namespace SiteServer.Cli.Updater.Model41
{
    public partial class TableTemplate
    {
        [JsonProperty("templateID")]
        public long TemplateId { get; set; }

        [JsonProperty("publishmentSystemID")]
        public long PublishmentSystemId { get; set; }

        [JsonProperty("templateName")]
        public string TemplateName { get; set; }

        [JsonProperty("templateType")]
        public string TemplateType { get; set; }

        [JsonProperty("relatedFileName")]
        public string RelatedFileName { get; set; }

        [JsonProperty("createdFileFullName")]
        public string CreatedFileFullName { get; set; }

        [JsonProperty("createdFileExtName")]
        public string CreatedFileExtName { get; set; }

        [JsonProperty("charset")]
        public string Charset { get; set; }

        [JsonProperty("isDefault")]
        public string IsDefault { get; set; }
    }

    public partial class TableTemplate
    {
        public const string OldTableName = "Template";

        public static readonly string NewTableName = DataProvider.TemplateDao.TableName;

        public static readonly List<TableColumnInfo> NewColumns = DataProvider.TemplateDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertDict =
            new Dictionary<string, string>
            {
                {nameof(TemplateInfo.Id), nameof(TemplateId)},
                {nameof(TemplateInfo.SiteId), nameof(PublishmentSystemId)}
            };
    }
}
