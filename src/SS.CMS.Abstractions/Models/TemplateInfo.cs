using SS.CMS.Data;
using SS.CMS.Enums;

namespace SS.CMS.Models
{
    [Table("siteserver_Template")]
    public class TemplateInfo : Entity
    {
        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string TemplateName { get; set; }

        [TableColumn]
        public string TemplateType { get; set; }

        public TemplateType Type
        {
            get => Enums.TemplateType.Parse(TemplateType);
            set => TemplateType = value.Value;
        }

        [TableColumn]
        public string RelatedFileName { get; set; }

        [TableColumn]
        public string CreatedFileFullName { get; set; }

        [TableColumn]
        public string CreatedFileExtName { get; set; }

        [TableColumn]
        public bool IsDefault { get; set; }

        public string Content { get; set; }
    }
}
