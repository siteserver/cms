using Datory;
using Datory.Annotations;

namespace SSCMS
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
        public bool DefaultTemplate { get; set; }

        [DataIgnore]
        public string Content { get; set; }
    }
}