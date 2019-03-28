using SiteServer.CMS.Database.Wrapper;

namespace SiteServer.CMS.Database.Models
{
    [Table("siteserver_RelatedFieldItem")]
    public class RelatedFieldItemInfo : DynamicEntity
    {
        [TableColumn]
        public int RelatedFieldId { get; set; }

        [TableColumn]
        public string ItemName { get; set; }

        [TableColumn]
        public string ItemValue { get; set; }

        [TableColumn]
        public int ParentId { get; set; }

        [TableColumn]
        public int Taxis { get; set; }
    }
}
