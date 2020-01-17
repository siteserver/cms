using Datory;
using Datory.Annotations;

namespace SiteServer.Abstractions
{
    [DataTable("siteserver_Template")]
    public class Template : Entity
    {
        [DataColumn] 
        public int SiteId { get; set; }

        [DataColumn] 
        public string TemplateName { get; set; }

        [DataColumn] 
        public TemplateType TemplateType { get; set; }

        [DataColumn] 
        public string RelatedFileName { get; set; }

        [DataColumn] 
        public string CreatedFileFullName { get; set; }

        [DataColumn] 
        public string CreatedFileExtName { get; set; }

        [DataColumn]
        [DataIgnore]
        private string IsDefault { get; set; }

        public bool Default
        {
            get => TranslateUtils.ToBool(IsDefault);
            set => IsDefault = value.ToString();
        }

        public string Content { get; set; }
    }
}