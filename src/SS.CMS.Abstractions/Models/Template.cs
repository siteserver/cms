using System;
using Newtonsoft.Json;
using SS.CMS.Data;
using SS.CMS.Enums;

namespace SS.CMS.Models
{
    [Serializable]
    [DataTable("siteserver_Template")]
    public class Template : Entity
    {
        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public string TemplateName { get; set; }

        [DataColumn]
        public string TemplateType { get; set; }

        [JsonIgnore]
        [DataIgnore]
        public TemplateType Type
        {
            get => Enums.TemplateType.Parse(TemplateType);
            set => TemplateType = value.Value;
        }

        [DataColumn]
        public string RelatedFileName { get; set; }

        [DataColumn]
        public string CreatedFileFullName { get; set; }

        [DataColumn]
        public string CreatedFileExtName { get; set; }

        [DataColumn]
        public bool IsDefault { get; set; }

        [DataIgnore]
        public string Content { get; set; }
    }
}
