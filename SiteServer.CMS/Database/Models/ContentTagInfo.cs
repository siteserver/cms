using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_ContentTag")]
    public class ContentTagInfo : DynamicEntity
    {
        [TableColumn]
        public string TagName { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int UseNum { get; set; }
    }
}
