using System.Collections.Generic;
using Newtonsoft.Json;
using SiteServer.CMS.Core;
using SiteServer.CMS.Model;
using SiteServer.Plugin;

namespace SiteServer.Cli.Updater.Model50
{
    public partial class TableTemplate
    {
        [JsonProperty("templateId")]
        public long TemplateId { get; set; }

        [JsonProperty("publishmentSystemId")]
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
        public bool IsDefault { get; set; }
    }

    public partial class TableTemplate
    {
        public const string OldTableName = "Template";

        public static readonly string NewTableName = DataProvider.TemplateDao.TableName;

        public static readonly List<TableColumn> NewColumns = DataProvider.TemplateDao.TableColumns;

        public static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(TemplateInfo.Id), nameof(TemplateId)},
                {nameof(TemplateInfo.SiteId), nameof(PublishmentSystemId)}
            };

        public static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
