using System.Collections.Generic;
using Newtonsoft.Json;
using SS.CMS.Models;
using SS.CMS.Repositories;

namespace SS.CMS.Cli.Updater.Tables
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

        [JsonProperty("ruleID")]
        public long RuleId { get; set; }

        [JsonProperty("charset")]
        public string Charset { get; set; }

        [JsonProperty("isDefault")]
        public string IsDefault { get; set; }
    }

    public partial class TableTemplate
    {
        public static readonly List<string> OldTableNames = new List<string>
        {
            "siteserver_Template",
            "wcm_Template"
        };

        public static ConvertInfo GetConverter(ITemplateRepository templateRepository) => new ConvertInfo
        {
            NewTableName = templateRepository.TableName,
            NewColumns = templateRepository.TableColumns,
            ConvertKeyDict = ConvertKeyDict,
            ConvertValueDict = ConvertValueDict
        };

        private static readonly Dictionary<string, string> ConvertKeyDict =
            new Dictionary<string, string>
            {
                {nameof(Template.Id), nameof(TemplateId)},
                {nameof(Template.SiteId), nameof(PublishmentSystemId)}
            };

        private static readonly Dictionary<string, string> ConvertValueDict = null;
    }
}
