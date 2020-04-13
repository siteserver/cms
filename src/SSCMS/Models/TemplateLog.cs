using Datory;
using Datory.Annotations;

namespace SSCMS.Models
{
    [DataTable("siteserver_TemplateLog")]
    public class TemplateLog : Entity
    {
        [DataColumn]
        public int TemplateId { get; set; }

        [DataColumn]
        public int SiteId { get; set; }

        [DataColumn]
        public int AdminId { get; set; }

        [DataColumn]
        public int ContentLength { get; set; }

        [DataColumn(Text = true)]
        public string TemplateContent { get; set; }
    }
}
