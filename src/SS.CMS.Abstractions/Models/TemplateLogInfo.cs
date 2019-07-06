using System;
using SS.CMS.Data;

namespace SS.CMS.Models
{
    [Serializable]
    [Table("siteserver_TemplateLog")]
    public class TemplateLogInfo : Entity
    {
        [TableColumn]
        public int TemplateId { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public string AddUserName { get; set; }

        [TableColumn]
        public int ContentLength { get; set; }

        [TableColumn(Text = true)]
        public string TemplateContent { get; set; }
    }
}
