using System;
using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_TemplateLog")]
    public class TemplateLogInfo : DynamicEntity
    {
        [TableColumn]
        public int TemplateId { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public DateTime? AddDate { get; set; }

        [TableColumn]
        public string AddUserName { get; set; }

        [TableColumn]
        public int ContentLength { get; set; }

        [TableColumn(Text = true)]
        public string TemplateContent { get; set; }
    }
}
