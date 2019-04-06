using Datory;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_ContentTag")]
    public class ContentTagInfo : Entity
    {
        [TableColumn]
        public string TagName { get; set; }

        [TableColumn]
        public int SiteId { get; set; }

        [TableColumn]
        public int UseNum { get; set; }
    }
}
